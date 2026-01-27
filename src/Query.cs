using System.Text;

namespace VoidNone.NoSQLite;

public class Query<T>
{
    private record OrderItem(string Selector, bool Descending);
    private long skip;
    private string[]? exclude;
    private string? owner;
    private readonly List<OrderItem> order = [];
    private readonly Connection connection;
    private readonly string name;
    private string? where;

    internal Query(Connection connection, string name)
    {
        this.connection = connection;
        this.name = name;
    }

    public Query<T> Where(string selector, object value)
    {
        Where(selector, Comparison.Equals, value);
        return this;
    }

    public Query<T> Where(string selector, Comparison comparison, object value)
    {
        string? condition = Query<T>.GetCondition(selector, comparison, value);
        if (!string.IsNullOrWhiteSpace(condition))
        {
            if (string.IsNullOrWhiteSpace(where))
            {
                where = condition;
            }
            else
            {
                where = $"({where} AND {condition})";
            }

        }
        return this;
    }

    public Query<T> OrWhere(string selector, object value)
    {
        OrWhere(selector, Comparison.Equals, value);
        return this;
    }

    public Query<T> OrWhere(string selector, Comparison comparison, object value)
    {
        string? condition = Query<T>.GetCondition(selector, comparison, value);
        if (!string.IsNullOrWhiteSpace(condition))
        {
            if (string.IsNullOrWhiteSpace(where))
            {
                where = condition;
            }
            else
            {
                where = $"({where} OR {condition})";
            }
        }
        return this;
    }

    public Query<T> Order(string selector)
    {
        order.Add(new OrderItem(selector, false));
        return this;
    }

    public Query<T> OrderByDescending(string selector)
    {
        order.Add(new OrderItem(selector, true));
        return this;
    }

    public Query<T> Skip(long count)
    {
        skip = count;
        return this;
    }

    public Query<T> Exclude(params string[] selectors)
    {
        exclude = selectors;
        return this;
    }

    public Query<T> OwnerIn(params string[] ids)
    {
        if (ids != null && ids.Length > 0)
        {
            owner = $"OwnerId IN ({string.Join(",", ids.Select(s => $"'{s}'"))})";
        }
        return this;
    }

    public Task<Document<T>[]> TakeAsync(CancellationToken token)
    {
        return TakeAsync(null, token);
    }

    public async Task<Document<T>[]> TakeAsync(long? count = null, CancellationToken token = default)
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder.Append($"SELECT rowid as RowId,Id,OwnerId,CreationTime,LastWriteTime,Enabled,Note,");

        if (exclude is null || exclude.Length == 0) sqlBuilder.Append("json(Data) as Data");
        else
        {
            var data = string.Join(',', exclude.Select(s => $"'{s}'"));
            sqlBuilder.Append($"json(jsonb_remove(Data,{data})) as Data");
        }

        sqlBuilder.AppendLine();
        sqlBuilder.AppendLine($"FROM `{name}`");
        sqlBuilder.AppendLine(GetWhere());

        if (order.Count > 0)
        {
            var orders = order.Select(s =>
            {
                var result = $"jsonb_extract(Data,'{s.Selector}')";

                if (s.Descending)
                {
                    result += " DESC";
                }

                return result;
            });

            sqlBuilder.AppendLine($"ORDER {string.Join(',', orders)}");
        }

        if (skip > 0)
        {
            sqlBuilder.AppendLine($"LIMIT {count ?? -1} OFFSET {skip}");
        }

        using var reader = connection.Query(sqlBuilder.ToString());
        var result = new List<Document<T>>();

        while (reader.Read())
        {
            var doc = await Collection<T>.ReadDocumentAsync(reader, token);
            result.Add(doc);
        }

        return [.. result];
    }

    public async Task<Document<T>?> FirstOrDefaultAsync()
    {
        var result = await TakeAsync(1);
        return result.FirstOrDefault();
    }

    public async Task<Document<T>> FirstAsync()
    {
        var result = await FirstOrDefaultAsync();
        return result ?? throw new DocumentNotFoundException();
    }

    public long Count()
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine($"SELECT COUNT(1) FROM `{name}`");
        sqlBuilder.AppendLine(GetWhere());

        if (skip > 0)
        {
            sqlBuilder.AppendLine($"LIMIT -1 OFFSET {skip}");
        }

        using var reader = connection.Query(sqlBuilder.ToString());
        reader.Read();
        return reader.GetInt64(0);
    }

    private string GetWhere()
    {
        var conditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(where))
        {
            conditions.Add($"({where})");
        }

        if (!string.IsNullOrWhiteSpace(owner))
        {
            conditions.Add($"({owner})");
        }

        if (conditions.Count == 0) return string.Empty;
        return $"WHERE {string.Join(" AND ", conditions)}";
    }
    private static string? GetCondition(string selector, Comparison comparison, object value)
    {
        selector = selector.Trim();
        // TODO
        // if (!selector.StartsWith("$.")) selector = $"$.{selector}";
        string? condition = null;
        selector = $"jsonb_extract(Data,'{selector}')";
        var valueInfo = GetValue(value);

        switch (comparison)
        {
            case Comparison.Equals:
                if (valueInfo.isNull)
                {
                    condition = $"{selector} IS NULL";
                }
                else
                {
                    condition = $"{selector} = {valueInfo.value}";
                }
                break;
            case Comparison.NotEquals:
                if (valueInfo.isNull)
                {
                    condition = $"{selector} IS NOT NULL";
                }
                else
                {
                    condition = $"{selector} != {valueInfo.value}";
                }
                break;
            case Comparison.Greater:
                condition = $"{selector} > {valueInfo.value}";
                break;
            case Comparison.GreaterOrEquals:
                condition = $"{selector} >= {valueInfo.value}";
                break;
            case Comparison.Less:
                condition = $"{selector} < {valueInfo.value}";
                break;
            case Comparison.LessOrEquals:
                condition = $"{selector} < {valueInfo.value}";
                break;
            case Comparison.StartsWith:
                condition = $"{selector} like '{value}%'";
                break;
            case Comparison.Contains:
                condition = $"{selector} like '%{value}%'";
                break;
            default:
                break;
        }

        return condition;
    }

    public static (string value, bool isNull) GetValue(object value)
    {
        if (value is null)
        {
            return ($"NULL", true);
        }
        else if (value is string)
        {
            return ($"'{value}'", false);
        }
        else
        {
            return (value.ToString()!, false);
        }
    }
}