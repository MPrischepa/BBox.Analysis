using System;
using System.Collections.Generic;
using BBox.Analysis.Domain.RecordTemplates.PaymentTemplates;

namespace BBox.Analysis.Domain.PaymentTypes
{
   
    public class Payment
    {
        public string PaymentTypeName => PaymentTypeDescr.Name;

        public String AdditionalInfo => ClientCard != null ? ClientCard.CardNo : String.Empty;

        public String CardNo => ClientCard == null ? String.Empty : ClientCard.CardNo;
        public ClientCard ClientCard { get; protected set; }
        public ClientType ClientType => PaymentTypeDescr.ClientType;

        public virtual PaymentType PaymentType => PaymentTypeDescr.PaymentType;

        public ClientCardType ClientCardType => ClientCard?.ClientCardType ?? ClientCardType.None;
 
        internal PaymentTypeDescription PaymentTypeDescr { get; }
        
        public Payment(PaymentTypeDescription descr)
        {
            PaymentTypeDescr = descr;
        }

        static Payment()
        {
            BlackBoxObject.AddTemplate(IgnoreRecordTemplate<Payment>.Instance);
            
        }

        public virtual void SetCardNo(String cardNo)
        {
            throw new Exception("Не должно быть карточки");
        }
    }
}
