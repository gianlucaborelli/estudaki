// InlineTagRenderer - Processa tags especiais dentro de TextInline
window.InlineTagRenderer = {

    /**
     * Processa todas as tags <math>...</math> dentro de um container
     * Substitui cada tag por um span renderizado com MathQuill
     */
    processMathTags: function (containerId) {
        const container = document.getElementById(containerId);
        if (!container) {
            console.warn(`Container ${containerId} não encontrado`);
            return;
        }

        console.log('Processando math tags para container:', containerId);
        console.log('HTML original:', container.innerHTML);

        // Regex para encontrar tags <math>...</math>
        // Usa flag 's' para permitir que . corresponda a quebras de linha
        const mathRegex = /<math>([\s\S]*?)<\/math>/gi;
        let html = container.innerHTML;
        let match;
        let replacements = [];

        // Encontrar todas as ocorrências
        while ((match = mathRegex.exec(html)) !== null) {
            const fullMatch = match[0];
            const latex = match[1].trim();
            const id = `math-inline-${this.generateId()}`;

            console.log('Encontrada tag math:', { fullMatch, latex, id });

            replacements.push({
                original: fullMatch,
                replacement: `<span class="math-inline-display" id="${id}"></span>`,
                latex: latex,
                id: id
            });
        }

        if (replacements.length === 0) {
            console.log('Nenhuma tag <math> encontrada');
            return;
        }

        // Aplicar substituições
        replacements.forEach(r => {
            html = html.replace(r.original, r.replacement);
        });

        container.innerHTML = html;
        console.log('HTML após substituições:', html);

        // Renderizar cada fórmula matemática
        replacements.forEach(r => {
            console.log('Renderizando math:', r.id, r.latex);
            if (window.MathQuillHelper) {
                window.MathQuillHelper.renderStatic(r.id, r.latex);
            } else {
                console.error('MathQuillHelper não está disponível');
            }
        });
    },

    /**
     * Processa todas as tags <chemical>...</chemical> dentro de um container
     * Substitui cada tag por um span renderizado com KaTeX/mhchem
     */
    processChemicalTags: function (containerId) {
        const container = document.getElementById(containerId);
        if (!container) {
            console.warn(`Container ${containerId} não encontrado`);
            return;
        }

        console.log('Processando chemical tags para container:', containerId);
        console.log('HTML original:', container.innerHTML);

        // Regex para encontrar tags <chemical>...</chemical>
        const chemicalRegex = /<chemical>([\s\S]*?)<\/chemical>/gi;
        let html = container.innerHTML;
        let match;
        let replacements = [];

        // Encontrar todas as ocorrências
        while ((match = chemicalRegex.exec(html)) !== null) {
            const fullMatch = match[0];
            const formula = match[1].trim();
            const id = `chemical-inline-${this.generateId()}`;

            console.log('Encontrada tag chemical:', { fullMatch, formula, id });

            replacements.push({
                original: fullMatch,
                replacement: `<span class="chemical-inline-display" id="${id}"></span>`,
                formula: formula,
                id: id
            });
        }

        if (replacements.length === 0) {
            console.log('Nenhuma tag <chemical> encontrada');
            return;
        }

        // Aplicar substituições
        replacements.forEach(r => {
            html = html.replace(r.original, r.replacement);
        });

        container.innerHTML = html;
        console.log('HTML após substituições:', html);

        // Renderizar cada fórmula química
        replacements.forEach(r => {
            console.log('Renderizando chemical:', r.id, r.formula);
            if (window.ChemicalHelper) {
                window.ChemicalHelper.renderStatic(r.id, r.formula);
            } else {
                console.error('ChemicalHelper não está disponível');
            }
        });
    },

    /**
     * Gera um ID único
     */
    generateId: function () {
        return Math.random().toString(36).substr(2, 9);
    }
};
