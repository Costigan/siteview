using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using siteview.views;

namespace siteview.dataset
{
    public interface IMapLayer : IDisposable
    {
        string Name { get; }
        float Transparency { get; set; }
        void Draw(Graphics g, MapView.DisplayTransform t);
        UserControl GetPropertySheet();

        /// <summary>
        /// Called when the layer is highighted or unhighlighted (mouse highlighted).
        /// Currently, only one layer can be highlighted.
        /// </summary>
        /// <param name="view"></param>
        void Selected(MapView view, bool isSelected);

        /// <summary>
        ///  Called when the layer's checkbox is changed (which changes whether the layer is rendered)
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isChecked"></param>
        void Checked(MapView view, bool isChecked);
    }

    public abstract class MapLayer : IMapLayer
    {
        public string Name { get; set; }
        protected float _Transparency = 1f;
        protected readonly ImageAttributes _imageAttributes = new ImageAttributes();  // Holds the image attributes so it can be reused
        protected ImageAttributes _drawAttributes = null;  // Either points at _imageAttributes or null
        protected readonly ColorMatrix _colorMatrix = new ColorMatrix();
        public float Transparency
        {
            get { return _Transparency; }
            set
            {
                _Transparency = value;
                if (_Transparency < 0f) _Transparency = 0f;
                if (_Transparency > 1f) _Transparency = 1f;
                if (_Transparency == 1f)
                    _drawAttributes = null;
                else
                {
                    _colorMatrix.Matrix33 = _Transparency;
                    _imageAttributes.SetColorMatrix(_colorMatrix);
                    _drawAttributes = _imageAttributes;
                }
            }
        }

        public override string ToString() => Name;

        public abstract void Draw(Graphics g, MapView.DisplayTransform t);

        public virtual UserControl GetPropertySheet() => null;

        public virtual void Selected(MapView view, bool isSelected) { }
        public virtual void Checked(MapView view, bool isChecked) { }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MapLayer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}
