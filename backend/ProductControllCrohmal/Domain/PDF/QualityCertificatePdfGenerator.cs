using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Repositories.Entities;

namespace Domain.Services.Services
{
    public static class QualityCertificatePdfGenerator
    {
        private static readonly CultureInfo UaCulture = new("uk-UA");

        public static byte[] Generate(
            QualityCertificate certificate,
            IReadOnlyList<AnalysisResult> results,
            IReadOnlyList<ProductQualitySpecification> specifications)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(24);

                    page.DefaultTextStyle(x => x
                        .FontFamily("Arial")
                        .FontSize(9));

                    page.Content().Element(content =>
                        ComposeCertificate(content, certificate, results, specifications));
                });
            }).GeneratePdf();
        }

        private static void ComposeCertificate(
            IContainer container,
            QualityCertificate certificate,
            IReadOnlyList<AnalysisResult> results,
            IReadOnlyList<ProductQualitySpecification> specifications)
        {
            container
                .Border(1.5f)
                .BorderColor(Colors.Black)
                .Padding(10)
                .Column(column =>
                {
                    column.Spacing(6);

                    column.Item().Element(x => ComposeHeader(x, certificate));
                    column.Item().Element(x => ComposeBatchInfo(x, certificate));

                    column.Item()
                        .PaddingTop(4)
                        .Text("ЯКІСНІ ПОКАЗНИКИ")
                        .FontSize(11)
                        .Bold()
                        .AlignCenter();

                    column.Item().Element(x => ComposeQualityTable(x, results, specifications));

                    column.Item().PaddingTop(8).Text(text =>
                    {
                        text.Span("ПРИМІТКА: ").Bold();
                        text.Span("Сертифікат сформовано автоматично в системі простежуваності партій продукції.");
                    });

                    column.Item().Element(x => ComposeFooter(x, certificate));
                });
        }

        private static void ComposeHeader(IContainer container, QualityCertificate certificate)
        {
            var productName = certificate.Batch?.Product?.Name ?? "—";

            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.ConstantItem(72)
                        .Height(64)
                        .Border(1)
                        .BorderColor(Colors.Black)
                        .AlignCenter()
                        .AlignMiddle()
                        .Column(logo =>
                        {
                            logo.Item().Text("КПУ")
                                .FontSize(18)
                                .Bold()
                                .AlignCenter();

                            logo.Item().Text("ЯКІСТЬ")
                                .FontSize(7)
                                .AlignCenter();
                        });

                    row.RelativeItem()
                        .PaddingLeft(8)
                        .Column(header =>
                        {
                            header.Item()
                                .Border(1)
                                .BorderColor(Colors.Black)
                                .Padding(4)
                                .Text("ТОВ «КРОХМАЛЕПРОДУКТИ УКРАЇНИ»")
                                .FontSize(13)
                                .Bold()
                                .AlignCenter();

                            header.Item()
                                .PaddingTop(5)
                                .Text("Сертифікат якості")
                                .FontSize(15)
                                .Bold()
                                .AlignCenter();

                            header.Item()
                                .Text(productName)
                                .FontSize(11)
                                .Bold()
                                .AlignCenter();

                            header.Item()
                                .Text("на партію продукції")
                                .FontSize(10)
                                .AlignCenter();

                            header.Item()
                                .Text($"№ сертифіката: {certificate.CertificateNumber}")
                                .FontSize(9)
                                .AlignCenter();
                        });
                });
            });
        }

        private static void ComposeBatchInfo(IContainer container, QualityCertificate certificate)
        {
            var batch = certificate.Batch;

            container.PaddingTop(8).Column(column =>
            {
                column.Spacing(5);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(x =>
                        ComposeInfoField(x, "Дата виготовлення", FormatDate(batch?.ProductionDate)));

                    row.ConstantItem(18);

                    row.RelativeItem().Element(x =>
                        ComposeInfoField(x, "№ партії", batch?.BatchNumber ?? "—"));
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(x =>
                        ComposeInfoField(x, "Виробнича лінія", batch?.ProductionLine ?? "—"));

                    row.ConstantItem(18);

                    row.RelativeItem().Element(x =>
                        ComposeInfoField(x, "Маса нетто", GetQuantityText(batch)));
                });

                column.Item().Element(x =>
                    ComposeInfoField(x, "Продукція", batch?.Product?.Name ?? "—"));
            });
        }

        private static void ComposeInfoField(
            IContainer container,
            string label,
            string value)
        {
            container.Row(row =>
            {
                row.ConstantItem(95)
                    .AlignMiddle()
                    .Text(label);

                row.RelativeItem()
                    .Element(Underline)
                    .AlignCenter()
                    .Text(value);
            });
        }

        private static void ComposeQualityTable(
            IContainer container,
            IReadOnlyList<AnalysisResult> results,
            IReadOnlyList<ProductQualitySpecification> specifications)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(28);
                    columns.RelativeColumn(3.4f);
                    columns.RelativeColumn(1.2f);
                    columns.RelativeColumn(1.2f);
                });

                table.Header(header =>
                {
                    AddHeaderCell(header, "№\nз/п", true);
                    AddHeaderCell(header, "Найменування показника", true);
                    AddHeaderCell(header, "Норма", true);
                    AddHeaderCell(header, "Значення", true);
                });

                if (results.Count == 0)
                {
                    table.Cell()
                        .ColumnSpan(4)
                        .Element(CellStyle)
                        .AlignCenter()
                        .Text("Результати лабораторного аналізу відсутні.");
                    return;
                }

                var orderedResults = results
                    .OrderBy(x => x.QualityParameter?.Name)
                    .ToList();

                for (var i = 0; i < orderedResults.Count; i++)
                {
                    var result = orderedResults[i];

                    AddBodyCell(table, (i + 1).ToString(), true);
                    AddBodyCell(table, result.QualityParameter?.Name ?? "—");
                    AddBodyCell(table, GetNormText(result, specifications), true);
                    AddBodyCell(table, GetResultValue(result), true);
                }
            });
        }

        private static void ComposeFooter(IContainer container, QualityCertificate certificate)
        {
            container.PaddingTop(12).Column(column =>
            {
                column.Spacing(8);

                column.Item().Text(text =>
                {
                    text.Span("Висновок ВТК: ").Bold();
                    text.Span(certificate.Conclusion);
                });

                column.Item().Row(row =>
                {
                    row.ConstantItem(110)
                        .Height(62)
                        .Border(1)
                        .BorderColor(Colors.Green.Darken2)
                        .AlignCenter()
                        .AlignMiddle()
                        .Column(stamp =>
                        {
                            stamp.Item().Text("М.П.")
                                .FontColor(Colors.Green.Darken2)
                                .Bold()
                                .AlignCenter();

                            stamp.Item().Text("ВТК")
                                .FontColor(Colors.Green.Darken2)
                                .FontSize(16)
                                .Bold()
                                .AlignCenter();
                        });

                    row.RelativeItem()
                        .PaddingLeft(18)
                        .Column(signature =>
                        {
                            signature.Item().Row(r =>
                            {
                                r.ConstantItem(115).Text("Начальник ВТК");
                                r.RelativeItem().BorderBottom(1).BorderColor(Colors.Black).Text("");
                            });

                            signature.Item().PaddingTop(12).Text(text =>
                            {
                                text.Span("Дата формування: ").Bold();
                                text.Span(certificate.CreatedAt.ToString("dd.MM.yyyy", UaCulture));
                            });

                            signature.Item().PaddingTop(4)
                                .Text("Документ сформовано автоматично на основі фінальної оцінки якості партії.")
                                .FontSize(8)
                                .Italic();
                        });
                });
            });
        }

        private static IContainer Underline(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Black)
                .PaddingBottom(1);
        }

        private static void AddHeaderCell(
            TableCellDescriptor header,
            string text,
            bool center = false)
        {
            var cell = header
                .Cell()
                .Element(HeaderCellStyle);

            if (center)
            {
                cell.AlignCenter().Text(text).Bold();
            }
            else
            {
                cell.Text(text).Bold();
            }
        }

        private static void AddBodyCell(
            TableDescriptor table,
            string text,
            bool center = false)
        {
            var cell = table
                .Cell()
                .Element(CellStyle);

            if (center)
            {
                cell.AlignCenter().Text(text);
            }
            else
            {
                cell.Text(text);
            }
        }

        private static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Black)
                .Background(Colors.Grey.Lighten3)
                .PaddingVertical(4)
                .PaddingHorizontal(3)
                .AlignMiddle();
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Black)
                .PaddingVertical(4)
                .PaddingHorizontal(3)
                .AlignMiddle();
        }

        private static string GetNormText(
            AnalysisResult result,
            IReadOnlyList<ProductQualitySpecification> specifications)
        {
            var specification = specifications.FirstOrDefault(x =>
                x.QualityParameterId == result.QualityParameterId);

            if (specification is null)
                return "—";

            var unit =
                specification.QualityParameter?.Unit ??
                result.QualityParameter?.Unit ??
                string.Empty;

            var hasMin = specification.MinValue.HasValue;
            var hasMax = specification.MaxValue.HasValue;

            if (hasMin && hasMax)
            {
                return $"{FormatDecimal(specification.MinValue!.Value)} – {FormatDecimal(specification.MaxValue!.Value)} {unit}".Trim();
            }

            if (hasMin && !hasMax)
            {
                return $"не менше {FormatDecimal(specification.MinValue!.Value)} {unit}".Trim();
            }

            if (!hasMin && hasMax)
            {
                return $"не більше {FormatDecimal(specification.MaxValue!.Value)} {unit}".Trim();
            }

            if (!string.IsNullOrWhiteSpace(specification.TextNorm))
            {
                return specification.TextNorm;
            }

            return "—";
        }

        private static string GetResultValue(AnalysisResult result)
        {
            var unit = result.QualityParameter?.Unit ?? string.Empty;

            if (result.NumericValue.HasValue)
            {
                return $"{FormatDecimal(result.NumericValue.Value)} {unit}".Trim();
            }

            if (!string.IsNullOrWhiteSpace(result.TextValue))
            {
                return result.TextValue;
            }

            return "—";
        }

        private static string FormatDate(DateTime? date)
        {
            return date.HasValue
                ? date.Value.ToString("dd.MM.yyyy", UaCulture)
                : "—";
        }

        private static string GetQuantityText(Batch? batch)
        {
            if (batch is null)
                return "—";

            return $"{FormatDecimal(batch.Quantity)} {batch.Unit}";
        }

        private static string FormatDecimal(decimal value)
        {
            return value.ToString("0.###", UaCulture);
        }
    }
}