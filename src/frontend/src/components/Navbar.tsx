import Link from "next/link"
import { SearchBar } from "./SearchBar"
import { LayoutDashboard, LineChart, PieChart } from "lucide-react"

export function Navbar() {
    return (
        <header className="sticky top-0 z-50 w-full border-b border-slate-800 bg-slate-950/80 backdrop-blur supports-[backdrop-filter]:bg-slate-950/60">
            <div className="container mx-auto flex h-16 items-center justify-between px-4">
                <div className="flex items-center gap-8">
                    <Link href="/" className="flex items-center gap-2 font-bold text-xl tracking-tighter hover:opacity-80 transition-opacity">
                        <div className="w-8 h-8 rounded-lg bg-gradient-to-tr from-cyan-500 to-blue-600 flex items-center justify-center shadow-lg shadow-cyan-900/20">
                            <LineChart className="h-5 w-5 text-white" />
                        </div>
                        <span className="bg-gradient-to-r from-white to-slate-400 bg-clip-text text-transparent">TwStock</span>
                    </Link>

                    <nav className="hidden md:flex items-center gap-6 text-sm font-medium text-slate-400">
                        <Link href="/screener" className="flex items-center gap-2 hover:text-cyan-400 transition-colors">
                            <LayoutDashboard className="h-4 w-4" />
                            選股器
                        </Link>
                        <Link href="/markets" className="flex items-center gap-2 hover:text-cyan-400 transition-colors">
                            <PieChart className="h-4 w-4" />
                            大盤概況
                        </Link>
                    </nav>
                </div>

                <div className="flex items-center gap-4">
                    <div className="hidden md:block w-96">
                        <SearchBar className="max-w-md h-9" />
                    </div>
                    <button className="text-sm font-medium text-slate-400 hover:text-white px-3 py-2 rounded-md hover:bg-slate-800 transition-all">
                        登入
                    </button>
                    <button className="text-sm font-medium bg-cyan-600 hover:bg-cyan-500 text-white px-4 py-2 rounded-full shadow-lg shadow-cyan-900/20 transition-all">
                        免費註冊
                    </button>
                </div>
            </div>
        </header>
    )
}
