const API_BASE_URL = 'http://localhost:5016/api';

export interface FinancialStatement {
    symbol: string;
    year: number;
    quarter: number;

    // Income Statement
    revenue: number;
    grossProfit: number;
    operatingIncome: number;
    netIncome: number;
    eps: number;

    // Balance Sheet
    totalAssets: number;
    totalLiabilities: number;
    totalEquity: number;
    cashAndCashEquivalents: number;

    // Cash Flow
    operatingCashFlow: number;
    capitalExpenditure: number;
    freeCashFlow: number;

    // Ratios
    roe: number;
    roa: number;
    grossMargin: number;
    operatingMargin: number;
    netMargin: number;
    debtToEquity: number;

    // Dividends
    dividends: number;
}

export interface StockSummary {
    symbol: string;
    name: string;
    industry: string;
    market: string;
    price: number | null;
    peRatio: number | null;
    dividendYield: number | null;
    pbRatio: number | null;
    roe: number | null;
    roa: number | null;
    eps: number | null;
}

export const api = {
    async getStockSummary(symbol: string): Promise<StockSummary | null> {
        try {
            const response = await fetch(`${API_BASE_URL}/stock/${symbol}`, { cache: 'no-store' });
            if (!response.ok) {
                console.error(`Error fetching stock summary: ${response.statusText}`);
                return null;
            }
            return response.json();
        } catch (error) {
            console.error("Failed to fetch stock summary:", error);
            return null;
        }
    },

    async getFinancials(symbol: string): Promise<FinancialStatement[]> {
        try {
            const response = await fetch(`${API_BASE_URL}/stock/${symbol}/financials`, { cache: 'no-store' });
            if (!response.ok) {
                console.error(`Error fetching financials: ${response.statusText}`);
                return [];
            }
            return response.json();
        } catch (error) {
            console.error("Failed to fetch financials:", error);
            return [];
        }
    }
};
