using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace XOProject.Controller
{
    [Route("api/Trade/")]
    public class TradeController : ControllerBase
    {
        private IShareRepository _shareRepository { get; set; }
        private ITradeRepository _tradeRepository { get; set; }
        private IPortfolioRepository _portfolioRepository { get; set; }

        public TradeController(IShareRepository shareRepository, ITradeRepository tradeRepository, IPortfolioRepository portfolioRepository)
        {
            _shareRepository = shareRepository;
            _tradeRepository = tradeRepository;
            _portfolioRepository = portfolioRepository;
        }


        [HttpGet("{portfolioid}")]
        public async Task<IActionResult> GetAllTradings([FromRoute]int portFolioid)
        {
            var trade = _tradeRepository.Query().Where(x => x.PortfolioId.Equals(portFolioid));
            return Ok(trade);
        }


        /// <summary>
        /// For a given symbol of share, get the statistics for that particular share calculating the maximum, minimum, 
        /// average and Sum of all the trades for that share individually grouped into Buy trade and Sell trade.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>

        [HttpGet("Analysis/{symbol}")]
        public async Task<IActionResult> GetAnalysis([FromRoute]string symbol)
        {
            var result = new List<TradeAnalysis>();
             TradeAnalysis ta = new TradeAnalysis();
            var tradeBuy = _tradeRepository.Query().Where(x => x.Symbol.Equals(symbol) && x.Action.Equals("BUY"));
            var tradeSell = _tradeRepository.Query().Where(x => x.Symbol.Equals(symbol) && x.Action.Equals("SELL"));

            List<int> trdNoSBuy = new List<int>();
            List<int> trdNoSSell = new List<int>();

            foreach (var t in tradeBuy)
            {
                trdNoSBuy.Add(t.NoOfShares);
            }
            foreach (var t in tradeSell)
            {
                trdNoSSell.Add(t.NoOfShares);
            }
            if (!trdNoSBuy.Count.Equals(0))
            {
                ta.Maximum = trdNoSBuy.Max();
                ta.Minimum = trdNoSBuy.Min();
                ta.Sum = trdNoSBuy.Sum();
                ta.Average = trdNoSBuy.Average();
                ta.Action = "BUY";
                result.Add(ta);
            }
            ta = new TradeAnalysis();
            if (!trdNoSSell.Count.Equals(0))
            {
                ta.Maximum = trdNoSSell.Max();
                ta.Minimum = trdNoSSell.Min();
                ta.Sum = trdNoSSell.Sum();
                ta.Average = trdNoSSell.Average();
                ta.Action = "SELL";
                result.Add(ta);
            }
            return Ok(result);
        }


    }
}
