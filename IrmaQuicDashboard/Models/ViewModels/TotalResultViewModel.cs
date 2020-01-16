using System;
using System.ComponentModel.DataAnnotations;
using IrmaQuicDashboard.Logic;

namespace IrmaQuicDashboard.Models.ViewModels
{
    public class TotalResultViewModel
    {
        // Aggregated data and averages
        public double QuicUsingWiFiNewRip { get; set; }
        public double QuicUsing3GStationaryNewRip { get; set; }
        public double QuicUsing4GMovingNewRip { get; set; }
        public double AverageQuicNewRip
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((QuicUsingWiFiNewRip + QuicUsing3GStationaryNewRip + QuicUsing4GMovingNewRip) / 3);
            }
        }

        public double TLSUsingWiFiNewRip { get; set; }
        public double TLSUsing3GStationaryNewRip { get; set; }
        public double TLSUsing4GMovingNewRip { get; set; }
        public double AverageTLSNewRip
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((TLSUsingWiFiNewRip + TLSUsing3GStationaryNewRip + TLSUsing4GMovingNewRip) / 3);
            }
        }

        public double PercentageDiffUsingWiFiNewRip
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((QuicUsingWiFiNewRip - TLSUsingWiFiNewRip) / TLSUsingWiFiNewRip * 100);
            }
        }
        public double PercentageDiffUsing3GStationaryNewRip
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((QuicUsing3GStationaryNewRip - TLSUsing3GStationaryNewRip) / TLSUsing3GStationaryNewRip * 100);
            }
        }
        public double PercentageDiffUsing4GMovingNewRip
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((QuicUsing4GMovingNewRip - TLSUsing4GMovingNewRip) / TLSUsing4GMovingNewRip * 100);
            }
        }
        public double AveragePercentageDiffNewRip
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((PercentageDiffUsingWiFiNewRip + PercentageDiffUsing3GStationaryNewRip + PercentageDiffUsing4GMovingNewRip)/3);
            }
        }

        public double QuicUsingWiFiResSuc { get; set; }
        public double QuicUsing3GStationaryResSuc { get; set; }
        public double QuicUsing4GMovingResSuc { get; set; }
        public double AverageQuicResSuc
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((QuicUsingWiFiResSuc + QuicUsing3GStationaryResSuc + QuicUsing4GMovingResSuc) / 3);
            }
        }

        public double TLSUsingWiFiResSuc { get; set; }
        public double TLSUsing3GStationaryResSuc { get; set; }
        public double TLSUsing4GMovingResSuc { get; set; }
        public double AverageTLSResSuc
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((TLSUsingWiFiResSuc + TLSUsing3GStationaryResSuc + TLSUsing4GMovingResSuc) / 3);
            }
        }

        public double PercentageDiffUsingWiFiResSuc
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((QuicUsingWiFiResSuc - TLSUsingWiFiResSuc) / TLSUsingWiFiResSuc * 100);
            }
        }
        public double PercentageDiffUsing3GStationaryResSuc
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((QuicUsing3GStationaryResSuc - TLSUsing3GStationaryResSuc) / TLSUsing3GStationaryResSuc * 100);
            }
        }
        public double PercentageDiffUsing4GMovingResSuc
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((QuicUsing4GMovingResSuc - TLSUsing4GMovingResSuc) / TLSUsing4GMovingResSuc * 100);
            }
        }
        public double AveragePercentageDiffResSuc
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((PercentageDiffUsingWiFiResSuc + PercentageDiffUsing3GStationaryResSuc + PercentageDiffUsing4GMovingResSuc) / 3);
            }
        }

        // Totals.
        [Display(Name = "Total upload sessions: ")]
        public double TotalUploadSessions { get; set; }

        [Display(Name = "Average of all QUIC sessions: ")]
        public double TotalAverageQUIC
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((QuicUsingWiFiNewRip +
                    QuicUsing3GStationaryNewRip +
                    QuicUsing4GMovingNewRip +
                    QuicUsingWiFiResSuc +
                    QuicUsing3GStationaryResSuc +
                    QuicUsing4GMovingResSuc) /6);
            }
        }

        [Display(Name = "Average of all TCP/TLS sessions: ")]
        public double TotalAverageTLS
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((TLSUsingWiFiNewRip +
                    TLSUsing3GStationaryNewRip +
                    TLSUsing4GMovingNewRip +
                    TLSUsingWiFiResSuc +
                    TLSUsing3GStationaryResSuc +
                    TLSUsing4GMovingResSuc) / 6);
            }
        }

        [Display(Name = "Total difference %: ")]
        public double TotalAveragePercentageDiff
        {
            get
            {
                return CalculationLogic.RoundToThreeDecimals((TotalAverageQUIC - TotalAverageTLS) / TotalAverageTLS * 100);
            }
        }
    }
}
