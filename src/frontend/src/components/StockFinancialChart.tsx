"use client"

import { useMemo } from 'react';
import {
    LineChart,
    Line,
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
    ResponsiveContainer,
    Area,
    ComposedChart
} from 'recharts';
import { FinancialStatement } from '@/services/api';

interface StockFinancialChartProps {
    data: FinancialStatement[];
}

export function StockFinancialChart({ data }: StockFinancialChartProps) {
    // Only show annual data (quarter === 0) and filter out invalid data
    const annualData = useMemo(() => {
        return data
            .filter(item => item.quarter === 0 && item.revenue > 0)
            .sort((a, b) => a.year - b.year)
            .map(item => ({
                ...item,
                year: item.year.toString(),
                // Convert to percentage for display
                roePercent: (item.roe * 100),
                roaPercent: (item.roa * 100),
                grossMarginPercent: (item.grossMargin * 100),
                operatingMarginPercent: (item.operatingMargin * 100),
                netMarginPercent: (item.netMargin * 100),
                // Convert to billions for display
                revenueBillion: item.revenue / 1_000_000_000,
                netIncomeBillion: item.netIncome / 1_000_000_000,
                grossProfitBillion: item.grossProfit / 1_000_000_000,
                operatingIncomeBillion: item.operatingIncome / 1_000_000_000,
                equityBillion: item.totalEquity / 1_000_000_000,
                freeCashFlowBillion: item.freeCashFlow / 1_000_000_000
            }));
    }, [data]);

    // Customize Tooltip
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const CustomTooltip = ({ active, payload, label }: any) => {
        if (active && payload && payload.length) {
            return (
                <div className="bg-slate-900/95 border border-slate-700 p-4 rounded-lg shadow-xl text-sm backdrop-blur-sm">
                    <p className="font-bold text-slate-200 mb-3 text-base">{label} 年報</p>
                    {payload.map((entry: any) => (
                        <div key={entry.name} style={{ color: entry.color }} className="flex justify-between gap-6 mb-1">
                            <span className="text-slate-300">{entry.name}:</span>
                            <span className="font-mono font-semibold">
                                {entry.name.includes('%')
                                    ? `${entry.value.toFixed(2)}%`
                                    : entry.name === 'EPS'
                                        ? entry.value.toFixed(2)
                                        : `${entry.value.toFixed(0)}億`}
                            </span>
                        </div>
                    ))}
                </div>
            );
        }
        return null;
    };

    return (
        <div className="space-y-6">
            {/* Revenue and Profit Chart */}
            <div className="w-full h-[400px] bg-gradient-to-br from-slate-900/40 to-slate-800/30 border border-slate-700/50 rounded-2xl p-6 shadow-xl">
                <h3 className="text-lg font-semibold text-slate-200 mb-4 flex items-center gap-2">
                    <span className="w-1 h-6 bg-blue-500 rounded"></span>
                    營收與獲利趨勢
                </h3>
                <ResponsiveContainer width="100%" height="90%">
                    <ComposedChart data={annualData} margin={{ top: 5, right: 30, left: 10, bottom: 5 }}>
                        <CartesianGrid strokeDasharray="3 3" stroke="#334155" opacity={0.3} />
                        <XAxis dataKey="year" stroke="#94a3b8" fontSize={12} />
                        <YAxis yAxisId="left" stroke="#94a3b8" fontSize={12} label={{ value: '億元', angle: -90, position: 'insideLeft', fill: '#94a3b8' }} />
                        <YAxis yAxisId="right" orientation="right" stroke="#94a3b8" fontSize={12} label={{ value: 'EPS', angle: 90, position: 'insideRight', fill: '#94a3b8' }} />
                        <Tooltip content={<CustomTooltip />} />
                        <Legend wrapperStyle={{ paddingTop: '20px' }} />

                        <Bar yAxisId="left" dataKey="revenueBillion" name="營收" fill="#3b82f6" opacity={0.8} />
                        <Bar yAxisId="left" dataKey="netIncomeBillion" name="淨利" fill="#10b981" opacity={0.9} />
                        <Line yAxisId="right" type="monotone" dataKey="eps" name="EPS" stroke="#f59e0b" strokeWidth={3} dot={{ r: 4, fill: '#f59e0b' }} />
                    </ComposedChart>
                </ResponsiveContainer>
            </div>

            {/* ROE, ROA, Margins Chart */}
            <div className="w-full h-[400px] bg-gradient-to-br from-slate-900/40 to-slate-800/30 border border-slate-700/50 rounded-2xl p-6 shadow-xl">
                <h3 className="text-lg font-semibold text-slate-200 mb-4 flex items-center gap-2">
                    <span className="w-1 h-6 bg-purple-500 rounded"></span>
                    獲利能力指標
                </h3>
                <ResponsiveContainer width="100%" height="90%">
                    <LineChart data={annualData} margin={{ top: 5, right: 30, left: 10, bottom: 5 }}>
                        <CartesianGrid strokeDasharray="3 3" stroke="#334155" opacity={0.3} />
                        <XAxis dataKey="year" stroke="#94a3b8" fontSize={12} />
                        <YAxis stroke="#94a3b8" fontSize={12} unit="%" />
                        <Tooltip content={<CustomTooltip />} />
                        <Legend wrapperStyle={{ paddingTop: '20px' }} />

                        <Line type="monotone" dataKey="roePercent" name="ROE%" stroke="#8b5cf6" strokeWidth={3} dot={{ r: 4 }} />
                        <Line type="monotone" dataKey="roaPercent" name="ROA%" stroke="#06b6d4" strokeWidth={3} dot={{ r: 4 }} />
                        <Line type="monotone" dataKey="grossMarginPercent" name="毛利率%" stroke="#10b981" strokeWidth={2} strokeDasharray="5 5" dot={{ r: 3 }} />
                        <Line type="monotone" dataKey="netMarginPercent" name="淨利率%" stroke="#f59e0b" strokeWidth={2} strokeDasharray="5 5" dot={{ r: 3 }} />
                    </LineChart>
                </ResponsiveContainer>
            </div>

            {/* Cash Flow and Equity Chart */}
            <div className="w-full h-[400px] bg-gradient-to-br from-slate-900/40 to-slate-800/30 border border-slate-700/50 rounded-2xl p-6 shadow-xl">
                <h3 className="text-lg font-semibold text-slate-200 mb-4 flex items-center gap-2">
                    <span className="w-1 h-6 bg-green-500 rounded"></span>
                    現金流與股東權益
                </h3>
                <ResponsiveContainer width="100%" height="90%">
                    <ComposedChart data={annualData} margin={{ top: 5, right: 30, left: 10, bottom: 5 }}>
                        <CartesianGrid strokeDasharray="3 3" stroke="#334155" opacity={0.3} />
                        <XAxis dataKey="year" stroke="#94a3b8" fontSize={12} />
                        <YAxis stroke="#94a3b8" fontSize={12} label={{ value: '億元', angle: -90, position: 'insideLeft', fill: '#94a3b8' }} />
                        <Tooltip content={<CustomTooltip />} />
                        <Legend wrapperStyle={{ paddingTop: '20px' }} />

                        <Bar dataKey="freeCashFlowBillion" name="自由現金流" fill="#14b8a6" opacity={0.8} />
                        <Line type="monotone" dataKey="equityBillion" name="股東權益" stroke="#8b5cf6" strokeWidth={3} dot={{ r: 4 }} />
                    </ComposedChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
}
