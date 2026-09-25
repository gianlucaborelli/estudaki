import sanitizeHtml from 'sanitize-html';

const sanitizeOptions: sanitizeHtml.IOptions = {
    allowedTags: sanitizeHtml.defaults.allowedTags.concat([
        'div',
        'span',
        'p',
        'br',
        'h1',
        'h2',
        'h3',
        'h4',
        'h5',
        'h6',
        'ol',
        'ul',
        'li',
        'blockquote',
        'pre',
        'code',
        'sub',
        'sup',
        's',
        'img',
        'iframe',
        'video',
        'audio',
        'source'
    ]),
    allowedAttributes: {
        ...sanitizeHtml.defaults.allowedAttributes,
        '*': ['class', 'style', 'dir', 'data-*']
    },
    allowedSchemes: ['http', 'https', 'mailto'],
    allowVulnerableTags: false
};

export function sanitizeQuestionContent(content: string): string {
    return sanitizeHtml(content, sanitizeOptions);
}