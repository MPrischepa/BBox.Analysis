using System;
using BBox.Analysis.Domain.RecordTemplates.PaymentTemplates;

namespace BBox.Analysis.Domain.PaymentTypes
{
    public class BonusCardPayment : Payment
    {
        static BonusCardPayment()
        {
            BlackBoxObject.AddTemplate(SetCardTemplate<BonusCardPayment>.Instance);
        }

        
        public BonusCardPayment(PaymentTypeDescription descr,IBonusCardFactory factory) : base(descr)
        {
            PaymentType = descr.PaymentType | PaymentType.BonusesSettlement; 
           _factory = factory;
        }

        private readonly IBonusCardFactory _factory;

        

        #region Overrides of Payment

        #region Overrides of Payment

        public override PaymentType PaymentType { get; }

        #endregion

        #endregion

        #region Implementation of IContainsClientCard

        
        public Decimal ChargeBonuses { get; private set; }

        public void AddChargeBonuses(FuelSale sale, decimal bonuses)
        {
            ChargeBonuses = ChargeBonuses + bonuses;
            ((BonusCard)ClientCard).AddChargeBonuses(sale, bonuses);
        }

        public void RemoveChargeBonuses(FuelSale sale, decimal bonuses)
        {
            ChargeBonuses = ChargeBonuses - bonuses;
            ((BonusCard)ClientCard).RemoveChargeBonuses(sale, bonuses);
        }

        public Decimal CancellationBonuses { get; private set; }

        public void AddCancellationBonuses(FuelSale sale, decimal bonuses)
        {
            CancellationBonuses = CancellationBonuses + bonuses;
            ((BonusCard)ClientCard).AddCancellationBonuses(sale, bonuses);
        }

        public void RemoveCancellationBonuses(FuelSale sale, decimal bonuses)
        {
            CancellationBonuses = CancellationBonuses - bonuses;
            ((BonusCard)ClientCard).RemoveCancellationBonuses(sale, bonuses);
        }
        public override void SetCardNo(string cardNo)
        {
            ClientCard = _factory.GetBonusCard(cardNo);
        }

        #endregion
    }
}
