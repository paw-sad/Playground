using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Shouldly;
using TransfersModule;
using TransfersModule.Contract;
using TransfersModule.Contract.Shared;

namespace Tests.TransfersModule
{
    [TestClass]
    public class MatchingExceptionTests
    {
        [TestInitialize]
        public async Task Setup()
        {
            var db = new MongoClient("mongodb://localhost:27017");
            await db.GetDatabase("transfers-module").DropCollectionAsync("transfers");
            await db.GetDatabase("transfers-module").DropCollectionAsync("transfer-instructions");
        }

        private readonly TransfersApi _api = new();

        public static IEnumerable<object[]> TestData()
        {
            // different contract start date
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2021, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new NoSalary()
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new NoSalary()
                }
            };
            // different contract end date
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2022, 01, 01),
                    Salary = new NoSalary()
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new NoSalary()
                }
            };
            // different salary type
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new NoSalary()
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryRegular()
                }
            };
            // regular salary type, different amount
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryRegular
                    {
                        CurrencyCode = "USD",
                        SalaryAmount = 100,
                        SalaryInterval = SalaryInterval.Monthly
                    }
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryRegular
                    {
                        CurrencyCode = "USD",
                        SalaryAmount = 200,
                        SalaryInterval = SalaryInterval.Monthly
                    }
                }
            };
            // regular salary type, different currency code
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryRegular
                    {
                        CurrencyCode = "USD",
                        SalaryAmount = 100,
                        SalaryInterval = SalaryInterval.Monthly
                    }
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryRegular
                    {
                        CurrencyCode = "CHF",
                        SalaryAmount = 100,
                        SalaryInterval = SalaryInterval.Monthly
                    }
                }
            };

            // regular salary type, different interval
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryRegular
                    {
                        CurrencyCode = "USD",
                        SalaryAmount = 100,
                        SalaryInterval = SalaryInterval.Monthly
                    }
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryRegular
                    {
                        CurrencyCode = "USD",
                        SalaryAmount = 100,
                        SalaryInterval = SalaryInterval.Yearly
                    }
                }
            };

            // irregular, one period different currency code
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            }
                        }
                    }
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "CHF",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            }
                        }
                    }
                }
            };

            // irregular, one period different amount
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            }
                        }
                    }
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 200,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            }
                        }
                    }
                },
            };
            // irregular, one period different start
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            }
                        }
                    }
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2021, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            }
                        }
                    }
                }
            };
            // irregular, one period different end
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            }
                        }
                    }
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2022, 01, 01),
                            }
                        }
                    }
                }
            };

            // irregular, duplicated period
            yield return new[]
            {
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            }
                        }
                    }
                },
                new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = new PlayersSalaryIrregular
                    {
                        SalaryPeriods = new List<IrregularSalaryPeriod>
                        {
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            },
                            new()
                            {
                                CurrencyCode = "USD",
                                SalaryAmount = 100,
                                PeriodStart =   new DateTime(2020, 01, 01),
                                PeriodEnd =  new DateTime(2021, 01, 01),
                            }
                        }
                    }
                }
            };
        }

        [TestMethod]
        [DynamicData(nameof(TestData), DynamicDataSourceType.Method)]
        public async Task Test_instructions_matching_when_there_are_differences_between_engaging_and_releasing_instruction_player_contract(
            PlayersContract engagingContract, PlayersContract releasingContract)
        {
            // given engaging instruction was already entered
            var engageRequest = new EngageWithTransferAgreement.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PlayersContract = engagingContract
            };

            await _api.Execute(engageRequest);
            
            // when entering releasing instruction that differs in Player Contract details
            var releaseRequest = new ReleasePlayerContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PlayersContract = releasingContract
            };

            var releaseResponse = await _api.Execute(releaseRequest);

            // then instructions should be paired into transfer with MatchingException state
            var transfer = await _api.Execute(new GetTransferByIdContract.Request(releaseResponse.TransferId.Value));

            transfer.State.ShouldBe(TransferState.MatchingException);
        }
    }
}