using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using BBox.Analysis.Core.Logger;


namespace BBox.Analysis.Domain.PaymentTypes
{
    public class PaymentFactory
    {
        private static PaymentFactory _instance;
        public static PaymentFactory Instance => _instance;

        private static readonly ILogger _logger;

        static PaymentFactory()
        {
            _logger = LogManager.GetInstance().GetLogger("BBox.Analysis");
        }
        public static void CreateInstance(IBonusCardFactory factory, IAccountCardFactory accountCardfactory)
        {
            _instance = new PaymentFactory(factory, accountCardfactory);
        }

        private readonly IBonusCardFactory _bonusCardfactory;
        private readonly IAccountCardFactory _accountCardfactory;
        private PaymentFactory(IBonusCardFactory factory, IAccountCardFactory accountCardfactory)
        {
            _bonusCardfactory = factory;
            _accountCardfactory = accountCardfactory;
            _payments = new Dictionary<string, PaymentConfig>();
        }

        public Payment CreatePayment(PaymentTypeDescription paymentDescr,ClientCardType clientCardType)
        {
            if (paymentDescr == null)
                throw new ArgumentNullException();

            switch (clientCardType)
            {
                    case ClientCardType.None:return new Payment(paymentDescr);
                    case ClientCardType.AccountCard: return new AccountCardPayment(paymentDescr,_accountCardfactory);
                    case ClientCardType.DiscountCard: return new DiscountCardPayment(paymentDescr);
                    case ClientCardType.BonusCard: return new BonusCardPayment(paymentDescr,_bonusCardfactory);
                default:
                    throw new ArgumentOutOfRangeException(nameof(clientCardType), clientCardType, null);
            }

            
        }

        private IDictionary<String, PaymentConfig>  _payments;

        public void Configure(IList<PaymentConfig> payments)
        {
            _payments = payments.ToDictionary(x => x.Name, x => x);
        }

        public Payment CreatePayment(String paymentType)
        {
            if (_payments.TryGetValue(paymentType, out var __config))
                return CreatePayment(new PaymentTypeDescription
                {
                    Name = __config.Name,
                    ClientType = __config.ClientType,
                    PaymentType = __config.PaymentType
                }, __config.ClientCardType);
            _logger.Error($"Не отработано основание оплаты: {paymentType}");
            throw new ArgumentException($"Не отработано основание оплаты: {paymentType}");

            //switch (paymentType)
            //{
            //    //в дальнейшем вынести в конфигурацию
            //    case "Наличный расчет":
            //    case "Роснефть":
            //    case "РосНефть":
            //    case "Скидка бензин":
            //    case "Подарочная карта":
            //    case "Подарочные карты":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashSettlement,
            //            Name = paymentType,
            //            ClientType = ClientType.PhysicalPerson
            //        },ClientCardType.None);
            //    case "Ведомость":
            //    case "ведомость":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashlessSettlement|PaymentType.Statement,
            //            Name = paymentType,
            //            ClientType = ClientType.LegalEntity
            //        }, ClientCardType.None);
            //    case "Регион":
            //    case "Ликард безнал":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashlessSettlement,
            //            Name = paymentType,
            //            ClientType = ClientType.LegalEntity
            //        },ClientCardType.None); 
                        
            //    case "VISA":
            //    case "VISA (ИП Муравьева)":
            //    case "Халва":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashlessSettlement,
            //            ClientType = ClientType.PhysicalPerson,
            //            Name = paymentType
            //        },ClientCardType.None);
            //    case "Диалог":
            //    case "диалог":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashlessSettlement,
            //            ClientType = ClientType.LegalEntity,
            //            Name = paymentType
            //        },ClientCardType.AccountCard);//new AccountCardPayment(new LegalEntityPayment(paymentType));
            //    case "VISA Переходи на газ!":
            //    case "VISA Переходи на ГАЗ!":
            //    case "VISA Перходи на газ!":
            //    case "VISA Переход на газ!":
            //    case "VISA ЛНР":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashlessSettlement,
            //            ClientType = ClientType.PhysicalPerson,
            //            Name = paymentType
            //        },ClientCardType.DiscountCard);//new DiscountCardPayment(new CashlessPhysicalPersonPayment(paymentType));
            //    case "ЛНР":
            //    case "Переходи на ГАЗ!":
            //    case "Переходи на Газ!":
            //    case "Переходи на газ!":
            //    case "Переходи на газ":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashSettlement,
            //            ClientType = ClientType.PhysicalPerson,
            //            Name = paymentType
            //        },ClientCardType.DiscountCard);//new DiscountCardPayment(new PhysicalPersonPayment(paymentType));
            //    case "Бонусы":
            //    case "бонусы":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashSettlement,
            //            ClientType = ClientType.PhysicalPerson,
            //            Name = paymentType
            //        }, ClientCardType.BonusCard);//new BonusCardPayment(new PhysicalPersonPayment(paymentType),_factory );
            //    case "VISA бонусы":
            //    case "VISA Бонусы":
            //    case "VISA Бонус":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashlessSettlement,
            //            ClientType = ClientType.PhysicalPerson,
            //            Name = paymentType
            //        }, ClientCardType.BonusCard);//new BonusCardPayment(new CashlessPhysicalPersonPayment(paymentType),_factory );
            //    case "Мерник ГАЗ":
            //    case "мерник ГАЗ":
            //    case "Мерник газ":
            //    case "Мерник":
            //    case "мерник":
            //    case "Скидка для сотрудников":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.WithoutPayment,
            //            ClientType = ClientType.Service,
            //            Name = paymentType
            //        },ClientCardType.None);//new ServicePayment(paymentType);
            //    case "Баллоны со скидкой":
            //        return CreatePayment(new PaymentTypeDescription
            //        {
            //            PaymentType = PaymentType.CashSettlement,
            //            ClientType = ClientType.Other,
            //            Name = paymentType
            //        },ClientCardType.None);//new OtherPayment(paymentType);
            //    default:
            //        _logger.Error($"Не отработано основание оплаты: {paymentType}");
            //        throw new ArgumentException($"Не отработано основание оплаты: {paymentType}");
            //}
        }
    }
}
