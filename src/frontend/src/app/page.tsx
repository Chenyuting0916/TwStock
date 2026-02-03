import { Navbar } from "@/components/Navbar";
import { SearchBar } from "@/components/SearchBar";
import { TrendingUp, ShieldCheck, Zap } from "lucide-react";

export default function Home() {
  return (
    <>
      <Navbar />
      <main className="flex-1 flex flex-col items-center justify-center relative overflow-hidden">
        {/* Background Gradients */}
        <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[1000px] h-[600px] bg-cyan-500/10 blur-[120px] rounded-full pointer-events-none" />
        <div className="absolute bottom-0 right-0 w-[800px] h-[600px] bg-blue-600/10 blur-[100px] rounded-full pointer-events-none" />

        <div className="container px-4 py-32 flex flex-col items-center text-center relative z-10">
          <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-slate-900/50 border border-slate-800 text-xs font-medium text-cyan-400 mb-8 backdrop-blur-sm">
            <span className="relative flex h-2 w-2">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-cyan-400 opacity-75"></span>
              <span className="relative inline-flex rounded-full h-2 w-2 bg-cyan-500"></span>
            </span>
            台股價值投資分析平台 Now Live
          </div>

          <h1 className="text-5xl md:text-7xl font-bold tracking-tight mb-6 max-w-4xl">
            <span className="bg-gradient-to-b from-white to-slate-400 bg-clip-text text-transparent">
              洞察價值，
            </span>
            <span className="text-cyan-400">決策未來</span>
          </h1>

          <p className="text-lg text-slate-400 max-w-2xl mb-12 leading-relaxed">
            TwStock 提供最完整的台股 10 年財報數據、視覺化圖表與價值評估模型。
            像頂尖投資者一樣思考，從基本面挖掘潛在價值。
          </p>

          <SearchBar className="h-16 text-lg shadow-2xl shadow-cyan-900/20 max-w-2xl w-full" />

          {/* Feature Grid */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 mt-24 w-full max-w-5xl">
            {[
              {
                icon: TrendingUp,
                title: "10年歷史數據",
                desc: "不再受限於短期視角，完整檢視企業長期競爭力。"
              },
              {
                icon: Zap,
                title: "極速財報分析",
                desc: "ROE, ROA, EPS 趨勢一目了然，秒速判斷體質。"
              },
              {
                icon: ShieldCheck,
                title: "價值選股器",
                desc: "自定義篩選條件，找出被市場低估的優質好股。"
              }
            ].map((f, i) => (
              <div key={i} className="flex flex-col items-center p-6 rounded-2xl bg-slate-900/50 border border-slate-800/50 hover:bg-slate-800/50 hover:border-slate-700 transition-all group cursor-pointer">
                <div className="w-12 h-12 rounded-full bg-slate-800 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform group-hover:bg-cyan-500/10">
                  <f.icon className="h-6 w-6 text-cyan-400" />
                </div>
                <h3 className="text-lg font-semibold text-white mb-2">{f.title}</h3>
                <p className="text-sm text-slate-400">{f.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </main>
    </>
  );
}
