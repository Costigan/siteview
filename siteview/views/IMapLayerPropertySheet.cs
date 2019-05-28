using siteview.dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace siteview.views
{
    public interface IMapLayerPropertySheet
    {
        IMapLayer Layer { get; set; }
    }
}
