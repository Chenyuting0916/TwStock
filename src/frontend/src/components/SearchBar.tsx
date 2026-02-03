"use client"

import { Search } from "lucide-react"
import { useRouter } from "next/navigation"
import { useState } from "react"
import { cn } from "@/lib/utils"

export function SearchBar({ className }: { className?: string }) {
    const router = useRouter()
    const [query, setQuery] = useState("")

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault()
        if (query.trim()) {
            router.push(`/company/${query.trim().toUpperCase()}`)
        }
    }

    return (
        <form
            onSubmit={handleSearch}
            className={cn("relative group w-full max-w-lg", className)}
        >
            <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400 ml-1 group-focus-within:text-cyan-400 transition-colors" />
                <input
                    type="text"
                    placeholder="搜尋股票代號 (例如: 2330)..."
                    className="w-full h-12 pl-12 pr-4 bg-slate-900/50 border border-slate-700/50 rounded-full text-sm placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50 focus:border-cyan-500/50 transition-all shadow-xl backdrop-blur-sm hover:bg-slate-900/80"
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                />
                <div className="absolute right-3 top-1/2 -translate-y-1/2 flex gap-1">
                    <span className="text-[10px] bg-slate-800 text-slate-400 px-1.5 py-0.5 rounded border border-slate-700 font-mono">TW</span>
                </div>
            </div>
        </form>
    )
}
