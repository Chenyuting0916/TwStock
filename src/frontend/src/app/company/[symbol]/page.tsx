import { Navbar } from "@/components/Navbar";
import { api } from "@/services/api";
import { StockFinancialChart } from "@/components/StockFinancialChart";

interface PageProps {
    params: { symbol: string };
}

export default async function CompanyPage({ params }: PageProps) {
    const { symbol } = params;

    // Fetch data in parallel
    const stockDataPromise = api.getStockSummary(symbol);
    const financialsPromise = api.getFinancials(symbol);

    // Handle errors or empty states gracefully if needed
    // For now simple await
    const [stock, financials] = await Promise.all([
        stockDataPromise,
        financialsPromise
    ]);

    // If stock not found or API failed
    if (!stock) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-slate-950 text-slate-400">
                <div className="text-center">
                    <h1 className="text-2xl font-bold mb-2">無法載入資料</h1>
                    <p>請確認後端伺服器已啟動，或檢查股票代號是否正確。</p>
                    <p className="text-xs mt-4 text-slate-600">Debug info: API call to {symbol} failed.</p>
                </div>
            </div>
        );
    }

    return (
        <>
            <Navbar />
            <main className="container mx-auto px-4 py-8">
                <div className="flex flex-col gap-6">
                    {/* Header Section */}
                    <div className="flex items-start justify-between">
                        <div>
                            <h1 className="text-3xl font-bold text-white flex items-center gap-3">
                                {stock.symbol}
                                <span className="text-lg text-slate-500 font-normal border border-slate-800 px-2 py-0.5 rounded">
                                    {stock.name}
                                </span>
                            </h1>
                            <p className="text-slate-400 mt-1">{stock.industry} | {stock.market}</p>
                            <div className="flex gap-2 mt-3">
                                <span className="px-2 py-1 bg-slate-800 rounded text-xs text-slate-300">
                                    {stock.industry}
                                </span>
                            </div>
                        </div>
                        <div className="text-right">
                            <div className="text-3xl font-mono font-bold text-cyan-400">
                                {stock.price?.toFixed(2) ?? "N/A"}
                            </div>
                            <div className="text-xs text-slate-500 mt-1">最新收盤價</div>
                        </div>
                    </div>

                    {/* Stats Grid */}
                    <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
                        {[
                            { label: "本益比 (P/E)", value: stock.peRatio ? `${stock.peRatio.toFixed(1)}x` : '-' },
                            { label: "股價淨值比 (P/B)", value: stock.pbRatio ? `${stock.pbRatio.toFixed(1)}x` : '-' },
                            { label: "殖利率 (Yield)", value: stock.dividendYield ? `${stock.dividendYield.toFixed(2)}%` : '-' },
                            { label: "ROE (最新)", value: stock.roe ? `${stock.roe.toFixed(1)}%` : '-' },
                            { label: "ROA (最新)", value: stock.roa ? `${stock.roa.toFixed(1)}%` : '-' },
                            { label: "EPS (最新)", value: stock.eps ? `${stock.eps.toFixed(2)}` : '-' },
                        ].map((stat, i) => (
                            <div key={i} className="bg-slate-900/50 p-4 rounded-xl border border-slate-800/50">
                                <div className="text-xs text-slate-500 mb-1">{stat.label}</div>
                                <div className="text-lg font-semibold text-slate-200">{stat.value}</div>
                            </div>
                        ))}
                    </div>

                    {/* Main Content Area */}
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-4">
                        {/* Charts Area */}
                        <div className="lg:col-span-2">
                            {/* Pass data to Client Component */}
                            <StockFinancialChart data={financials} />
                        </div>

                        {/* Sidebar Info */}
                        <div className="bg-slate-900/30 border border-slate-800/50 rounded-2xl p-6 h-fit">
                            <h2 className="text-lg font-semibold mb-4">關於公司</h2>
                            <p className="text-sm text-slate-400 leading-relaxed">
                                {stock.name} ({stock.symbol}) 屬於 {stock.industry} 產業...
                                (此處可擴充更多公司簡介資料)
                            </p>
                        </div>
                    </div>
                </div>
            </main>
        </>
    )
}

