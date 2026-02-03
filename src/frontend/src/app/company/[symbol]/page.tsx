import { Navbar } from "@/components/Navbar";

export default function CompanyPage({ params }: { params: { symbol: string } }) {
    return (
        <>
            <Navbar />
            <main className="container mx-auto px-4 py-8">
                <div className="flex flex-col gap-6">
                    {/* Header Section */}
                    <div className="flex items-start justify-between">
                        <div>
                            <h1 className="text-3xl font-bold text-white flex items-center gap-3">
                                {params.symbol} <span className="text-lg text-slate-500 font-normal border border-slate-800 px-2 py-0.5 rounded">台積電</span>
                            </h1>
                            <p className="text-slate-400 mt-1">Taiwan Semiconductor Manufacturing Co., Ltd.</p>
                            <div className="flex gap-2 mt-3">
                                <span className="px-2 py-1 bg-slate-800 rounded text-xs text-slate-300">半導體</span>
                                <span className="px-2 py-1 bg-slate-800 rounded text-xs text-slate-300">TSE</span>
                            </div>
                        </div>
                        <div className="text-right">
                            <div className="text-3xl font-mono font-bold text-cyan-400">585.00</div>
                            <div className="text-sm text-green-400 mt-1 flex justify-end items-center gap-1">
                                +12.00 (+2.09%)
                            </div>
                            <div className="text-xs text-slate-500 mt-1">2026-02-03 收盤</div>
                        </div>
                    </div>

                    {/* Stats Grid */}
                    <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
                        {[
                            { label: "本益比 (P/E)", value: "18.5x" },
                            { label: "股價淨值比 (P/B)", value: "4.2x" },
                            { label: "殖利率 (Yield)", value: "2.5%" },
                            { label: "ROE (近四季)", value: "24.5%" },
                            { label: "ROA (近四季)", value: "15.2%" },
                            { label: "市值", value: "15.2T" },
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
                        <div className="lg:col-span-2 bg-slate-900/30 border border-slate-800/50 rounded-2xl p-6 min-h-[400px]">
                            <h2 className="text-lg font-semibold mb-4">財務趨勢 (10年)</h2>
                            <div className="flex items-center justify-center h-full text-slate-500">
                                Chart Placeholder (Recharts Coming Soon)
                            </div>
                        </div>

                        {/* Sidebar Info */}
                        <div className="bg-slate-900/30 border border-slate-800/50 rounded-2xl p-6">
                            <h2 className="text-lg font-semibold mb-4">關於公司</h2>
                            <p className="text-sm text-slate-400 leading-relaxed">
                                台灣積體電路製造公司（台積電）是全球最大的專用半導體代工廠...
                            </p>
                        </div>
                    </div>
                </div>
            </main>
        </>
    )
}
