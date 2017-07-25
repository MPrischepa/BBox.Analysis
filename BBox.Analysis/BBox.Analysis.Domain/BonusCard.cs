using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Domain.PaymentTypes;

namespace BBox.Analysis.Domain
{
    public class BonusCard :ClientCard
    {
        public BonusCard(String cardNo):base(cardNo)
        {
           _movements = new List<Movement>();
        }

        public Decimal Bonuses { get; private set; }

        private readonly IList<Movement> _movements;
        public IEnumerable<Movement> Movements => _movements;

        public void AddChargeBonuses(FuelSale sale,decimal bonuses)
        {
            _movements.Add(new Movement
            {
                Operation = BonusesOperation.ChargeBonuses,
                OperationDate = sale.Date,
                Sale = sale,
                Bonuses = bonuses
            });
            Bonuses = Bonuses + bonuses;
        }

        public void RemoveChargeBonuses(FuelSale sale, decimal bonuses)
        {
            _movements.Add(new Movement
            {
                Operation = BonusesOperation.ChargeBonuses,
                OperationDate = sale.Date,
                Sale = sale,
                Bonuses = -bonuses
            });
            Bonuses = Bonuses - bonuses;
        }

        public void AddCancellationBonuses(FuelSale sale,decimal bonuses)
        {
            _movements.Add(new Movement
            {
                Operation = BonusesOperation.CancellationBonuses,
                OperationDate = sale.Date,
                Sale = sale,
                Bonuses = bonuses
            });
            Bonuses = Bonuses - bonuses;
        }

        public void RemoveCancellationBonuses(FuelSale sale, decimal bonuses)
        {
            _movements.Add(new Movement
            {
                Operation = BonusesOperation.CancellationBonuses,
                OperationDate = sale.Date,
                Sale = sale,
                Bonuses = -bonuses
            });
            Bonuses = Bonuses + bonuses;
        }

        #region Overrides of ClientCard

        public override ClientCardType ClientCardType  => ClientCardType.BonusCard;

        #endregion
    }

    public class Movement
    {
        public DateTime OperationDate { get; set; }
        public FuelSale Sale { get; set; }

        public BonusesOperation Operation { get; set; }
        public decimal Bonuses { get; set; }
    }

    public enum BonusesOperation
    {
        ChargeBonuses,
        CancellationBonuses
    }

   
}
