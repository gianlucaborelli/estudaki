using System.Text;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.Services;

/// <summary>
/// Serviço responsável pela migração de conteúdo em bloco e inline para formato HTML compatível com Quill.
/// </summary>
public class ContentMigrationService
{
    /// <summary>
    /// Converte um InlineContent para HTML.
    /// </summary>
    public string ConvertInlineToHtml(InlineContent inline)
    {
        return inline switch
        {
            TextInline textInline => ConvertTextInlineToHtml(textInline),
            ImageInline imageInline => ConvertImageInlineToHtml(imageInline),
            MathInline mathInline => ConvertMathInlineToHtml(mathInline),
            ChemicalFormulaInline chemicalInline => ConvertChemicalFormulaToHtml(chemicalInline),
            _ => string.Empty
        };
    }

    /// <summary>
    /// Converte um ContentBlock para HTML.
    /// </summary>
    public string ConvertContentBlockToHtml(ContentBlock block)
    {
        return block switch
        {
            ParagraphBlock paragraphBlock => ConvertParagraphBlockToHtml(paragraphBlock),
            ImageBlock imageBlock => ConvertImageBlockToHtml(imageBlock),
            _ => string.Empty
        };
    }

    /// <summary>
    /// Converte uma lista de ContentBlocks para HTML.
    /// </summary>
    public string ConvertContentBlocksToHtml(List<ContentBlock>? blocks)
    {
        if (blocks == null || blocks.Count == 0)
            return string.Empty;

        var html = new StringBuilder();
        var orderedBlocks = blocks.OrderBy(b => b.Order).ToList();

        foreach (var block in orderedBlocks)
        {
            html.Append(ConvertContentBlockToHtml(block));
        }

        return html.ToString();
    }

    /// <summary>
    /// Converte uma lista de InlineContent para HTML.
    /// </summary>
    public string ConvertInlinesToHtml(List<InlineContent>? inlines)
    {
        if (inlines == null || inlines.Count == 0)
            return string.Empty;

        var html = new StringBuilder();

        foreach (var inline in inlines)
        {
            html.Append(ConvertInlineToHtml(inline));
        }

        return html.ToString();
    }

    /// <summary>
    /// Migra QuestionContents para Statement em formato HTML.
    /// </summary>
    public string MigrateQuestionContentsToStatement(List<ContentBlock>? questionContents)
    {
        return ConvertContentBlocksToHtml(questionContents);
    }

    /// <summary>
    /// Migra Choice Content/ContentBlocks para Explanation em formato HTML.
    /// </summary>
    public string MigrateChoiceContentToExplanation(Choice choice)
    {
        // Tenta migrar de ContentBlocks primeiro (mais recente)
        if (choice.ContentBlocks != null && choice.ContentBlocks.Count > 0)
        {
            return ConvertContentBlocksToHtml(choice.ContentBlocks);
        }

        // Se não houver ContentBlocks, tenta migrar de Content (antigo)
        if (choice.Content != null && choice.Content.Count > 0)
        {
            return ConvertInlinesToHtml(choice.Content);
        }

        return string.Empty;
    }

    /// <summary>
    /// Migra QuestionSupport Contents para Content em formato HTML.
    /// </summary>
    public string MigrateQuestionSupportContentsToContent(List<ContentBlock>? contents)
    {
        return ConvertContentBlocksToHtml(contents);
    }

    private string ConvertTextInlineToHtml(TextInline textInline)
    {
        if (string.IsNullOrEmpty(textInline.Text))
            return string.Empty;

        var text = textInline.Text;

        if (textInline.Bold)
            text = $"<strong>{text}</strong>";

        if (textInline.Italic)
            text = $"<em>{text}</em>";

        return text;
    }

    private string ConvertImageInlineToHtml(ImageInline imageInline)
    {
        if (string.IsNullOrEmpty(imageInline.Key))
            return string.Empty;

        var alt = string.IsNullOrEmpty(imageInline.Alt) ? imageInline.Key : imageInline.Alt;
        var widthAttr = imageInline.Width > 0 ? $" width=\"{imageInline.Width}\"" : string.Empty;
        var heightAttr = imageInline.Height > 0 ? $" height=\"{imageInline.Height}\"" : string.Empty;

        return $"<img src=\"{imageInline.Key}\" alt=\"{alt}\"{widthAttr}{heightAttr} />";
    }

    private string ConvertMathInlineToHtml(MathInline mathInline)
    {
        if (string.IsNullOrEmpty(mathInline.Latex))
            return string.Empty;

        // Usa notação LaTeX com delimitadores $ para compatibilidade com Quill/MathQuill
        return $"<span class=\"math-inline\">${mathInline.Latex}$</span>";
    }

    private string ConvertChemicalFormulaToHtml(ChemicalFormulaInline chemicalInline)
    {
        if (string.IsNullOrEmpty(chemicalInline.Formula))
            return string.Empty;

        // Usa notação mhchem com delimitadores $$ce{} para renderização
        return $"<span class=\"chem-formula\">$$ce{{{chemicalInline.Formula}}}$$</span>";
    }

    private string ConvertParagraphBlockToHtml(ParagraphBlock paragraphBlock)
    {
        var content = new StringBuilder();

        // Adiciona título se existir
        if (!string.IsNullOrEmpty(paragraphBlock.Title))
        {
            content.Append($"<h3>{paragraphBlock.Title}</h3>");
        }

        // Tenta usar o texto direto primeiro (mais recente)
        if (!string.IsNullOrEmpty(paragraphBlock.Text))
        {
            content.Append($"<p>{paragraphBlock.Text}</p>");
        }
        // Se não houver texto direto, tenta converter de Inlines
        else if (paragraphBlock.Inlines != null && paragraphBlock.Inlines.Count > 0)
        {
            var inlinesHtml = ConvertInlinesToHtml(paragraphBlock.Inlines);
            if (!string.IsNullOrEmpty(inlinesHtml))
            {
                content.Append($"<p>{inlinesHtml}</p>");
            }
        }

        // Adiciona source/fonte se existir
        if (!string.IsNullOrEmpty(paragraphBlock.Source))
        {
            content.Append($"<small class=\"source\">{paragraphBlock.Source}</small>");
        }

        return content.ToString();
    }

    private string ConvertImageBlockToHtml(ImageBlock imageBlock)
    {
        if (string.IsNullOrEmpty(imageBlock.Key) && string.IsNullOrEmpty(imageBlock.Url))
            return string.Empty;

        var src = !string.IsNullOrEmpty(imageBlock.Url) ? imageBlock.Url : imageBlock.Key;
        var alt = !string.IsNullOrEmpty(imageBlock.Title) ? imageBlock.Title : imageBlock.Key;
        var figure = new StringBuilder();

        figure.Append("<figure>");
        figure.Append($"<img src=\"{src}\" alt=\"{alt}\" />");

        if (!string.IsNullOrEmpty(imageBlock.Title))
        {
            figure.Append($"<figcaption><strong>{imageBlock.Title}</strong></figcaption>");
        }

        if (!string.IsNullOrEmpty(imageBlock.Description))
        {
            figure.Append($"<figcaption>{imageBlock.Description}</figcaption>");
        }

        if (!string.IsNullOrEmpty(imageBlock.Source))
        {
            figure.Append($"<figcaption><small>{imageBlock.Source}</small></figcaption>");
        }

        figure.Append("</figure>");

        return figure.ToString();
    }
}
