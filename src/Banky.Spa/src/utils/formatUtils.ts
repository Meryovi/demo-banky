const currencyFormat = Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

export const formatAmount = (amount: number) => currencyFormat.format(amount);
