using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace poupeai_report_service.Models;

internal class ExpenseReportModel : BaseReportModel
{
    [BsonElement("total_expense")]
    [BsonRepresentation(BsonType.Double)]
    public double TotalExpense { get; set; }


    [BsonElement("categories")]
    [BsonRepresentation(BsonType.Array)]
    public List<CategoryModel> Categories { get; set; } = [];

    [BsonElement("main_expenses")]
    [BsonRepresentation(BsonType.Array)]
    public List<TransactionModel> MainExpenses { get; set; } = [];
}