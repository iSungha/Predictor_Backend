-- Ensure there are no duplicates
DELETE FROM Prices
WHERE ctid NOT IN (
    SELECT MIN(ctid)
    FROM Prices
    GROUP BY Year, Month, ItemName, Price
);
