using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace poupeai_report_service.Models;

internal class IncomeReportModel : BaseReportModel
{
    [BsonElement("total_income")]
    [BsonRepresentation(BsonType.Double)]
    public double TotalIncome { get; set; }

    [BsonElement("categories")]
    [BsonRepresentation(BsonType.Array)]
    public List<CategoryModel> Categories { get; set; } = [];

    [BsonElement("main_incomes")]
    [BsonRepresentation(BsonType.Array)]
    public List<TransactionModel> MainIncomes { get; set; } = [];
}