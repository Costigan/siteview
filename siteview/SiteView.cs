using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using siteview.dataset;
using siteview.spice;
using siteview.views;
using ZedGraph;

namespace siteview
{
    public partial class SiteView : Form
    {
        public static SiteView Singleton;
        public readonly MapView MapView;
        public string SiteRoot = null;  // Directory that holds the sites
        public List<IMapLayer> Layers = new List<IMapLayer>();
        public SpiceManager Spice;
        public SiteDataset Dataset;

        private readonly ZedGraphControl renderPlot;

        protected bool _isUpdating;
        protected bool _trackbarIsUpdating;
        Dictionary<IMapLayer, UserControl> _layerPropertySheets = new Dictionary<IMapLayer, UserControl>();

        terrain.InMemoryTerrainManager _Terrain;

        public SiteView()
        {
            InitializeComponent();
            MapView = new MapView { Dock = DockStyle.Fill, MainWindow = this };
            tabMap.Controls.Add(MapView);
            renderPlot = new ZedGraphControl { Dock = DockStyle.Fill };
            pnlPlot.Controls.Add(renderPlot);

            Singleton = this;
            Spice = SpiceManager.GetSingleton();
            LoadSiteList();
        }

        #region Initialization

        void LoadSiteList()
        {
            SiteRoot = System.Configuration.ConfigurationManager.AppSettings["SiteRoot"];
            var names = Directory.GetDirectories(SiteRoot);
            var items = new ToolStripMenuItem[names.Length];
            for (var i = 0; i < names.Length; i++)
            {
                items[i] = new ToolStripMenuItem { Text = Path.GetFileName(names[i]), Name = $"site{i}" };
                items[i].Click += new EventHandler(SiteSelectionEvent);
            }
            siteToolStripMenuItem.DropDownItems.AddRange(items);
        }

        void SiteView_Load(object sender, EventArgs e)
        {
            if (siteToolStripMenuItem.DropDownItems.Count < 1)
                return;
            LoadFirstSite();
        }

        void LoadFirstSite()
        {
            if (siteToolStripMenuItem.DropDownItems.Count <= 0)
                return;
            var site_name = siteToolStripMenuItem.DropDownItems[0].Text;
            LoadDataset(site_name);
        }

        #endregion

        #region Methods

        void LoadDataset(string name)
        {
            FlushDataset();
            var dir = Path.Combine(SiteRoot, name);
            if (!Directory.Exists(dir))
                throw new Exception($"The site directory for {name} doesn't exist");
            Dataset = new SiteDataset(dir);
            UpdateFromLayers(Dataset.Layers);
            MapView.Transform = new MapView.DisplayTransform { OffsetX = 0, OffsetY = 0, Scale = 1f };
        }

        void FlushDataset()
        {
            Dataset?.Dispose();
            Dataset = null;
            lvLayers.Items.Clear();
        }


        #endregion

        #region Visibility

        internal void ShowRenderPlot(bool v)
        {
            pnlPlot.Visible = true;
            renderPlot.Visible = true;
        }

        #endregion

        #region Event Handlers

        void SiteSelectionEvent(object sender, EventArgs e)
        {
            var items = siteToolStripMenuItem.DropDownItems;
            for (var i = 0; i < items.Count; i++)
            {
                if (!(items[i] is ToolStripMenuItem item)) continue;
                item.Checked = false;
            }
            if (!(sender is ToolStripMenuItem item1)) return;
            item1.Checked = true;

            LoadDataset(item1.Text);
        }

        internal void GetLatLon(float y, float x, out double lat, out double lon)
        {
            lat = 0d;
            lon = 0d;
        }

        private void test1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new SiteDataset(@"C:\RP\tiles\sp\landing_site_datasets\site_001");
        }

        private void tbTransparency_ValueChanged(object sender, EventArgs e)
        {
            if (_trackbarIsUpdating)
                return;
            if (lvLayers.SelectedIndices.Count != 1)
                return;
            var lt = GetSelectedLayer();
            if (lt != null)
            {
                lt.Transparency = tbTransparency.Value / 100f;
                var index = lvLayers.SelectedIndices[0];
                lvLayers.Items[index].SubItems[1].Text = tbTransparency.Value.ToString("00");
                MapView.Invalidate();
            }
        }

        #endregion

        #region Handle Layers

        private void lvLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdating)
                return;
            if (lvLayers.SelectedIndices.Count < 1)
                return;
            _trackbarIsUpdating = true;
            var layer = LayerFromItem(lvLayers.SelectedItems[0]);
            var transparency = layer.Transparency;
            tbTransparency.Value = (int)(100 * transparency);
            UpdatePropertySheet(layer);
            _trackbarIsUpdating = false;
        }

        public IMapLayer LayerFromItem(ListViewItem item) => item.Tag as IMapLayer;

        private void lvLayers_SizeChanged(object sender, EventArgs e)
        {
            lvLayers.Columns[0].Width = lvLayers.Width - lvLayers.Columns[1].Width - 1;
        }

        private void lvLayers_KeyDown(object sender, KeyEventArgs e)
        {
            if (lvLayers.SelectedIndices.Count < 1)
                return;
            var index = lvLayers.SelectedIndices[0];
            if (e.KeyCode == Keys.Up && index > 0)
            {
                var item1 = lvLayers.Items[index];
                var item2 = lvLayers.Items[index - 1];
                var text = item1.Text;
                var isChecked = item1.Checked;
                var tag = item1.Tag;
                item1.Text = item2.Text;
                item1.Checked = item2.Checked;
                item1.Tag = item2.Tag;
                item2.Text = text;
                item2.Checked = isChecked;
                item2.Tag = tag;
                lvLayers.SelectedIndices.Clear();
                lvLayers.SelectedIndices.Add(index - 1);
                lvLayers.Invalidate();
            }
            else if (e.KeyCode == Keys.Down && index < lvLayers.Items.Count - 1)
            {
                var tmp = lvLayers.Items[index];
                lvLayers.Items.RemoveAt(index);
                lvLayers.Items.Insert(index + 1, tmp);
                lvLayers.Items[index + 1].Selected = true;
                lvLayers.Invalidate();
            }
            UpdateMapView();
            var temp = lvLayers.SelectedIndices;
        }

        private void lvLayers_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Tag is MapLayer layer)
                layer.Checked(MapView, e.Item.Checked);
            if (_isUpdating)
                return;
            UpdateMapView();
        }

        public void UpdateFromLayers(List<IMapLayer> layers)
        {
            _isUpdating = true;
            var currentItems = GetItems();
            var currentLayers = GetAllLayers();

            var layersToAdd = layers.Where(l => !currentLayers.Contains(l)).ToList();
            var layersToRemove = currentLayers.Where(l => !layers.Contains(l)).ToList();

            var items = lvLayers.Items;
            for (var i = items.Count - 1; i >= 0; i--)
                if (layersToRemove.Contains(LayerFromItem(items[i])))
                    items.RemoveAt(i);

            foreach (var toAdd in layersToAdd)
            {
                var subitems = new string[] { toAdd.Name, (toAdd.Transparency * 100f).ToString("00") };
                items.Add(new ListViewItem(subitems) { Name = toAdd.Name, Tag = toAdd });
            }

            if (!GetItems().Exists(item => item.Checked))
            {
                var first = GetItems().FirstOrDefault();
                if (first != null)
                    first.Checked = true;
            }

            _isUpdating = false;
            UpdateMapView();
        }

        protected MapLayer GetLayer(string name)
        {
            foreach (var item in lvLayers.Items)
                if (name.Equals(((item as ListViewItem).Tag as MapLayer)?.Name))
                    return (item as ListViewItem).Tag as MapLayer;
            return null;
        }

        /// <summary>
        /// Return the ListViewItems bottom to top (draw order)
        /// </summary>
        /// <returns></returns>
        public List<ListViewItem> GetItems()
        {
            var result = new List<ListViewItem>();
            var items = lvLayers.Items;
            for (var i = items.Count - 1; i >= 0; i--)
                result.Add(items[i]);
            return result;
        }

        public List<ListViewItem> GetCheckedItems()
        {
            var result = new List<ListViewItem>();
            var items = lvLayers.Items;
            for (var i = items.Count - 1; i >= 0; i--)
                if (items[i].Checked)
                    result.Add(items[i]);
            return result;
        }

        public List<IMapLayer> GetAllLayers() => GetItems().Select(LayerFromItem).Where(l => l != null).ToList();
        protected void UpdateMapView()
        {
            if (MapView == null) return;
            var layers = GetLayersToDraw();
            Console.WriteLine($"UpdateMapView layers={CommaSeparated(layers.Select(l => l.Name))}");
            MapView.Layers = layers;
        }

        public List<IMapLayer> GetLayersToDraw() => GetCheckedItems().Select(LayerFromItem).Where(l => l != null).ToList();
        public IMapLayer GetSelectedLayer() => lvLayers.SelectedItems.Count < 1 ? null : lvLayers.Items[lvLayers.SelectedIndices[0]].Tag as IMapLayer;

        private void UpdatePropertySheet(IMapLayer layer)
        {
            if (!_layerPropertySheets.TryGetValue(layer, out UserControl sheet))
            {
                sheet = layer.GetPropertySheet();
                if (sheet == null)
                {
                    foreach (var child in pnlPropertiesHolder.Controls)
                        if (child is UserControl c)
                            c.Visible = false;
                    return;
                }
                if (!(sheet is IMapLayerPropertySheet sheet1))
                {
                    MessageBox.Show($"layer returned an object that isn't an IMapLayerPropertySheet");
                    return;
                }
                _layerPropertySheets.Add(layer, sheet);
                sheet1.Layer = layer;
            }
            if (pnlPropertiesHolder != sheet.Parent)
                pnlPropertiesHolder.Controls.Add(sheet);
            foreach (var child in pnlPropertiesHolder.Controls)
                if (child is UserControl c)
                    c.Visible = c == sheet;
        }

        #endregion

        #region Utilities

        protected string CommaSeparated(IEnumerable<string> items)
        {
            string r = null;
            foreach (var item in items)
            {
                if (r == null)
                    r = item;
                else
                    r = r + ", " + item;
            }
            return r;
        }

        #endregion

        #region Terrain

        public terrain.InMemoryTerrainManager Terrain
        {
            get
            {
                if (_Terrain != null)
                    return _Terrain;
                _Terrain = new terrain.InMemoryTerrainManager();
                _Terrain.LoadSouth();
                return _Terrain;
            }
        }

        #endregion

        private void ControlsPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void LegendToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void PropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
