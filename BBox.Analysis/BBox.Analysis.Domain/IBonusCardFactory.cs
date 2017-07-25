using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain
{
    public interface IBonusCardFactory
    {
        BonusCard GetBonusCard(String cardNo);
    }
}
