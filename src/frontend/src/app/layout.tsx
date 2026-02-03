import type { Metadata } from "next";
import { Inter } from "next/font/google"; // Using Inter for clean, premium look
import "./globals.css";
import { cn } from "@/lib/utils";

const inter = Inter({ subsets: ["latin"], variable: "--font-inter" });

export const metadata: Metadata = {
  title: "TwStock | Value Investing Platform",
  description: "Taiwan Stock Analysis for Value Investors",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="zh-TW" className="dark">
      <body className={cn(inter.variable, "min-h-screen bg-slate-950 font-sans antialiased text-slate-100 selection:bg-cyan-500/30")}>
        <div className="relative flex min-h-screen flex-col">
          {children}
        </div>
      </body>
    </html>
  );
}
