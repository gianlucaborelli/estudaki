/**
 * ChemicalHelper - Helper para renderização de fórmulas químicas usando KaTeX + mhchem
 * 
 * Utiliza a extensão mhchem do KaTeX para renderizar fórmulas químicas.
 * Exemplos: H2O, CO2, C6H12O6, H2SO4, etc.
 */
window.ChemicalHelper = {
    /**
     * Renderiza uma fórmula química em um elemento HTML usando KaTeX + mhchem
     * @param {string} elementId - ID do elemento HTML onde a fórmula será renderizada
     * @param {string} formula - Fórmula química em notação mhchem (ex: "H2O", "CO2")
     * @param {number} retryCount - Número de tentativas restantes
     */
    renderStatic: function(elementId, formula, retryCount = 3) {
        try {
            const element = document.getElementById(elementId);

            if (!element) {
                if (retryCount > 0) {
                    setTimeout(() => {
                        this.renderStatic(elementId, formula, retryCount - 1);
                    }, 100);
                    return;
                } else {
                    return;
                }
            }

            element.innerHTML = '';

            if (!formula || formula.trim() === '') {
                element.innerHTML = '<span class="text-muted fst-italic"> </span>';
                return;
            }

            if (typeof katex === 'undefined') {
                console.error('ChemicalHelper: KaTeX não está carregado');
                element.innerHTML = '<span class="text-danger">Erro: KaTeX não carregado</span>';
                return;
            }

            // A sintaxe \ce{...} é específica do mhchem para fórmulas químicas
            const latexFormula = `\\ce{${formula}}`;

            katex.render(latexFormula, element, {
                throwOnError: false,
                displayMode: false,
                trust: true,
                strict: false,
                maxExpand: 1000, // Aumentar limite de expansões
                maxSize: 500,    // Aumentar tamanho máximo
                macros: {}       // Macros vazios (mhchem já está registrado globalmente)
            });
        } catch (error) {
            const element = document.getElementById(elementId);
            if (element) {
                element.innerHTML = `<span class="text-danger">Erro: ${error.message}</span>`;
            }
        }
    },

    /**
     * Verifica se o KaTeX e mhchem estão disponíveis
     * @returns {boolean} - true se estiverem disponíveis
     */
    isAvailable: function() {
        return typeof katex !== 'undefined';
    }
};

if (typeof katex !== 'undefined') {
    try {
        const testDiv = document.createElement('div');
        testDiv.style.display = 'none';
        document.body.appendChild(testDiv);
        katex.render('\\ce{H2O}', testDiv, { 
            throwOnError: true,
            trust: true,
            maxExpand: 1000
        });
        document.body.removeChild(testDiv);
    } catch (error) {
        console.error('ChemicalHelper: ERRO ao testar mhchem:', error);
        console.error('O mhchem pode não estar carregado ou configurado corretamente');
    }
} else {
    console.warn('ChemicalHelper: KaTeX não está disponível');
}
