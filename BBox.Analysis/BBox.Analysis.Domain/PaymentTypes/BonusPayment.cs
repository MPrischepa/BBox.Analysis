using System;
using BBox.Analysis.Domain.RecordTemplates.PaymentTemplates;

namespace BBox.Analysis.Domain.PaymentTypes
{
    //public class BonusPayment: Payment, IContainsClientCard
    //{
    //    static BonusPayment()
    //    {
    //        BlackBoxObject.AddTemplate(SetCardTemplate<BonusPayment>.Instance);
    //    }
    //    public BonusPayment(string paymentName,IBonusCardFactory factory) : base(paymentName)
    //    {
    //        _factory = factory;
    //    }

    //    private readonly IBonusCardFactory _factory;

    //    private BonusCard _bonusCard;

    //    #region Overrides of Payment

    //    public override string AdditionalInfo => CardNo;

    //    #endregion

    //    #region Implementation of IContainsClientCard

    //    public string CardNo => _bonusCard.CardNo;

    //    public Decimal ChargeBonuses { get; private set; }

    //    public void AddChargeBonuses(FuelSale sale,decimal bonuses)
    //    {
    //        ChargeBonuses = ChargeBonuses + bonuses;
    //        _bonusCard.AddChargeBonuses(sale,bonuses);
    //    }

    //    public void RemoveChargeBonuses(FuelSale sale, decimal bonuses)
    //    {
    //        ChargeBonuses = ChargeBonuses - bonuses;
    //        _bonusCard.RemoveChargeBonuses(sale,bonuses);
    //    }

    //    public Decimal CancellationBonuses { get; private set; }

    //    public void AddCancellationBonuses(FuelSale sale, decimal bonuses)
    //    {
    //        CancellationBonuses = CancellationBonuses + bonuses;
    //        _bonusCard.AddCancellationBonuses(sale,bonuses);
    //    }

    //    public void RemoveCancellationBonuses(FuelSale sale, decimal bonuses)
    //    {
    //        CancellationBonuses = CancellationBonuses - bonuses;
    //        _bonusCard.RemoveCancellationBonuses(sale,bonuses);
    //    }
    //    public void SetCardNo(string cardNo)
    //    {
    //        _bonusCard = _factory.GetBonusCard(cardNo);
    //    }

    //    #endregion
    //}

    
}
