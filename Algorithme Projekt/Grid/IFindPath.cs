using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grid
{
    public interface IFindPath
    {
        List<Cell> FindPath(Cell statingCell, Cell goalCell);
    }
}
