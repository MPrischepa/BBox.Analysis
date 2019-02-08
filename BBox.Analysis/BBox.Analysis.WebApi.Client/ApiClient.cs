using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Core.Logger;
using BBox.Analysis.Domain;
using BBox.Analysis.Interface;
using BBox.Analysis.WebApi.Client.Models;
using Newtonsoft.Json;
using ILogger = BBox.Analysis.Core.Logger.ILogger;

namespace BBox.Analysis.WebApi.Client
{
    public class ApiClient : IPostProcessing
    {
        public static string _jsonContentType = "application/json";

        private readonly string _apiBaseAddress;

        private readonly ILogger _logger;
        //private readonly bool _systemLogin = false;
        //private readonly string _systemToken;

        public ApiClient() : this(ConfigurationManager.AppSettings["ApiBaseAddress"])
        {
        }

        public ApiClient(string apiBaseAddress) : this(apiBaseAddress, LogManager.GetInstance().GetLogger("ApiClient"))
        {
        }

        public ApiClient(string apiBaseAddress, ILogger logger)
        {
            _apiBaseAddress = apiBaseAddress;
            _logger = logger;
        }

        //public ApiClient(bool systemLogin) : this()
        //{
        //    _systemLogin = systemLogin;

        //    if (!systemLogin)
        //        return;

        //    var login = Login("SystemUser", "wispa").Result;

        //    if (login.success)
        //        _systemToken = login.result;
        //}

        public static ApiClient Create()
        {
            return new ApiClient();
        }

        //public string SystemToken => _systemLogin ? _systemToken : "";

        //public async Task<WorkParameterSet> GetWorkParameterSet(Guid workParameterSetId, string workSetName, string token)
        //{
        //    var url = $"iwebapi/idsro/atp/wps/get?token={token}";
        //    var requestWps = new RequestWpsDto { WpsGuid = workParameterSetId, WpsName = workSetName };

        //    var defaultValue = new WorkParameterSet();
        //    var result = await PostAsync(url, requestWps, defaultValue);

        //    return result;
        //}

        private async Task<TResult> PostAsync<TBody, TResult>(string url, TBody body,
            TResult defaultValue = default(TResult))
        {
            using (var __client = CreateClient())
            {
                try
                {
                    var __serializeParameters = JsonConvert.SerializeObject(body);
                    var __response = await __client.PostAsync(url,
                        new StringContent(__serializeParameters, Encoding.UTF8, _jsonContentType));
                    __response.EnsureSuccessStatusCode();
                    var __resultStr = await __response.Content.ReadAsStringAsync();

                    var __result = JsonConvert.DeserializeObject<TResult>(__resultStr);

                    return __result;
                }
                catch (Exception __ex)
                {
                    _logger.Error($"Error request URL: {url}{Environment.NewLine}", __ex);
                    return defaultValue;
                }
            }
        }

        private HttpClient CreateClient()
        {
            var __client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(600),
                BaseAddress = new Uri(_apiBaseAddress)
            };
            __client.DefaultRequestHeaders.Accept.Clear();
            __client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_jsonContentType));
            return __client;
        }

        //public async Task<LoginResponseDto> Login(string userName, string password)
        //{
        //    var url = "iwebapi/login/";
        //    var defaultValue = new LoginResponseDto { success = false, result = "unknown error" };

        //    var formContent = new FormUrlEncodedContent(new[]
        //    {
        //        new KeyValuePair<string, string>("userName", userName),
        //        new KeyValuePair<string, string>("password", password)
        //    });

        //    return await PostAsync(url, formContent, defaultValue);
        //}

        //public async Task Logoff(string token)
        //{
        //    using (var client = CreateClient())
        //    {
        //        try
        //        {
        //            var response = await client.GetAsync(string.Format("iwebapi/logoff?token=" + token));
        //            response.EnsureSuccessStatusCode();
        //            SessionWrapper.UnregisterSessionInfoStatisticData();
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.ErrorException("Logoff", ex);
        //        }
        //    }
        //}

        private async Task<ResponseDTO> SetFuelStations(ICollection<FuelStation> stations)
        {
            var __url = "webapi/bbox/fuelstations";
            var __request = new
            {
                FuelStations = stations.Select(fs => new
                {
                    ExternalID = fs.Name,
                    FuelColumns = fs.FuelColumns.Select(fc => new
                    {
                        ExternalID = fc.ID,
                        FuelHoses = fc.FuelHoses.Select(fh => new
                        {
                            ExternalID = fh.Name
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return await PostAsync(__url, __request,
                new ResponseDTO {Success = false, Result = "unknown error"});
        }

        private async Task<ResponseDTO> SetShift(Shift shift)
        {
            var __url = "webapi/bbox/shifts";
            var __request = new
            {
                FuelStationExternalID = shift.FuelStation.Name,
                shift.BeginDate,
                shift.EndDate,
                StartHoseCounters = shift.StartShiftHoses.Select(x => new
                {
                    FuelColumnExternalID = x.FuelColumn,
                    FuelHoseExternalID = x.FuelHose,
                    Counter = x.Value
                }).ToList(),
                FinishedHoseCounters = shift.FinishedShiftHoses.Select(x => new
                {
                    FuelColumnExternalID = x.FuelColumn,
                    FuelHoseExternalID = x.FuelHose,
                    Counter = x.Value
                }).ToList(),
                Sales = shift.FuelsSales.Select(x => new
                {
                    ExternalID = x.ID,
                    StartCounter = x.StartCounterValue,
                    FinishedCounter = x.FinishedCounterValue,
                    SaleDate = x.Date,
                    x.CheckAmount,
                    x.IsCheckPrinted,
                    x.PourState,
                    x.SaleState,
                    FuelColumnExternalID = x.FuelHouse?.FuelColumn.ID,
                    FuelHoseExternalID = x.FuelHouse?.Name,
                    Payment = x.Payment == null
                        ? null
                        : new
                        {
                            x.Payment?.PaymentTypeName,
                            x.Payment?.PaymentType,
                            x.Payment?.ClientType,
                            x.Payment?.ClientCardType,
                            x.Payment?.CardNo
                        },
                    PreOrder = x.PreOrder == null
                        ? null
                        : new
                        {
                            FuelColumnExternalID = x.PreOrder?.FuelColumn.ID,
                            x.PreOrder?.ProductName,
                            x.PreOrder?.Price,
                            x.PreOrder?.DoseString,
                            x.PreOrder?.Volume,
                            x.PreOrder?.Amount,
                            Payment = x.PreOrder.Payment == null
                                ? null
                                : new
                                {
                                    x.PreOrder?.Payment.PaymentTypeName,
                                    x.PreOrder?.Payment.PaymentType,
                                    x.PreOrder?.Payment.ClientType,
                                    x.PreOrder?.Payment.ClientCardType,
                                    x.PreOrder?.Payment.CardNo
                                }
                        },
                    FactPour = x.FactPour == null
                        ? null
                        : new
                        {
                            FuelColumnExternalID = x.FactPour?.FuelColumn.ID,
                            x.FactPour?.ProductName,
                            x.FactPour?.Price,
                            x.FactPour?.DoseString,
                            x.FactPour?.Volume,
                            x.FactPour?.Amount,
                            Payment = x.FactPour.Payment == null
                                ? null
                                : new
                                {
                                    x.FactPour?.Payment.PaymentTypeName,
                                    x.FactPour?.Payment.PaymentType,
                                    x.FactPour?.Payment.ClientType,
                                    x.FactPour?.Payment.ClientCardType,
                                    x.FactPour?.Payment.CardNo
                                }
                        },
                    FactSale = x.FactSale == null
                        ? null
                        : new
                        {
                            FuelColumnExternalID = x.FactSale?.FuelColumn.ID,
                            x.FactSale?.ProductName,
                            x.FactSale?.Price,
                            x.FactSale?.DoseString,
                            x.FactSale?.Volume,
                            x.FactSale?.Amount,
                            Payment = x.FactSale.Payment == null
                                ? null
                                : new
                                {
                                    x.FactSale?.Payment.PaymentTypeName,
                                    x.FactSale?.Payment.PaymentType,
                                    x.FactSale?.Payment.ClientType,
                                    x.FactSale?.Payment.ClientCardType,
                                    x.FactSale?.Payment.CardNo
                                }
                        }
                }).ToList(),
                IncorrectConclusions = shift.Incorrect.Select(x => new {x.Date, x.PositionNo}).ToList()
            };
            return await PostAsync(__url, __request,
                new ResponseDTO {Success = false, Result = "unknown error"});
        }

        private async Task<ResponseDTO> SetBonusCard(BonusCard bonusCard)
        {
            var __url = "webapi/bbox/bonuscards";
            var __request = new
            {
                BonusCardNo = bonusCard.CardNo,
                Movements = bonusCard.Movements.Select(x => new
                {
                    FuelSaleID = x.Sale.ID,
                    FuelStationID = x.Sale.FuelStation.Name,
                    SaleDate = x.Sale.Date,
                    x.Operation,
                    x.OperationDate,
                    x.Bonuses
                }).ToList()
            };
            return await PostAsync(__url, __request,
                new ResponseDTO {Success = false, Result = "unknown error"});
        }

        private async Task<ResponseDTO> SetAccountCard(AccountCard accountCard)
        {
            var __url = "webapi/bbox/accountcards";
            var __request = new
            {
                AccountCardNo = accountCard.CardNo,
                AcceptedPayments = accountCard.AcceptedPayments.Select(x => new
                {
                    FuelSaleID = x.Sale.ID,
                    FuelStationID = x.Sale.FuelStation.Name,
                    SaleDate = x.Sale.Date,
                    x.ProductName,
                    x.OperationDate,
                    x.Price,
                    x.Amount,
                    x.Volume
                }).ToList()
            };
            return await PostAsync(__url, __request,
                new ResponseDTO { Success = false, Result = "unknown error" });
        }

        #region Implementation of IPostProcessing

        public void AfterProcessFinished(IDictionary<string, FuelStation> stations,
            IDictionary<string, BonusCard> bonusCards, IDictionary<string, AccountCard> accountCards,
            IList<Tuple<string, long, string>> invalidRecords)
        {
            var __fuelStationSyncResponse = SetFuelStations(stations.Values);
            __fuelStationSyncResponse.Wait();
            if (!__fuelStationSyncResponse.Result.Success)
            {
                _logger.Error($"Sync FuelStations Error: {__fuelStationSyncResponse.Result.Result}");
                return;
            }

            Parallel.ForEach(stations.Values.SelectMany(x => x.Shifts), () => (Task<ResponseDTO>) null,
                async (shift, state, arg3, arg4) =>
                {
                    try
                    {
                        return await SetShift(shift);

                    }
                    catch (Exception __e)
                    {
                        _logger.Error($"Sync Shift Error: {__e}");
                        return new ResponseDTO {Success = false, Result = $"{__e}"};
                    }

                }, dto =>
                {
                    if (dto == null) return;
                    dto.Wait();
                    if (!dto.Result.Success) _logger.Error($"Sync Shifts Error: {dto.Result.Result}");
                });

            Parallel.ForEach(bonusCards.Values, () => (Task<ResponseDTO>) null, async (bonusCard, state, arg3, arg4) =>
            {
                try
                {
                    return await SetBonusCard(bonusCard);

                }
                catch (Exception __e)
                {
                    _logger.Error($"Sync BonusCard Error: {__e}");
                    return new ResponseDTO {Success = false, Result = $"{__e}"};
                }

            }, dto =>
            {
                if (dto == null) return;
                dto.Wait();
                if (!dto.Result.Success) _logger.Error($"Sync BonusCards Error: {dto.Result.Result}");
            });

            Parallel.ForEach(accountCards.Values, () => (Task<ResponseDTO>)null, async (accountCard, state, arg3, arg4) =>
            {
                try
                {
                    return await SetAccountCard(accountCard);

                }
                catch (Exception __e)
                {
                    _logger.Error($"Sync BonusCard Error: {__e}");
                    return new ResponseDTO { Success = false, Result = $"{__e}" };
                }

            }, dto =>
            {
                if (dto == null) return;
                dto.Wait();
                if (!dto.Result.Success) _logger.Error($"Sync BonusCards Error: {dto.Result.Result}");
            });


        }

        #endregion
    }
}
