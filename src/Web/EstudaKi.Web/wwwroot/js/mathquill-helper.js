// MathQuill Helper para integração com Blazor
window.MathQuillHelper = {
    // Renderiza uma fórmula matemática estática (apenas visualização)
    renderStatic: function(elementId, latex) {
        const element = document.getElementById(elementId);
        if (!element) {
            console.error(`Elemento ${elementId} não encontrado`);
            return;
        }

        // Limpar conteúdo anterior
        element.innerHTML = '';

        if (!latex || latex.trim() === '') {
            element.innerHTML = '<span class="text-muted"> </span>';
            return;
        }

        try {
            // Verificar se MathQuill está disponível
            if (typeof MathQuill === 'undefined') {
                console.error('MathQuill não está carregado');
                element.innerHTML = '<span class="text-danger">MathQuill não carregado</span>';
                return;
            }

            const MQ = MathQuill.getInterface(2);

            // Criar elemento para MathQuill
            const mathField = document.createElement('span');
            element.appendChild(mathField);

            // Renderizar como campo estático (não editável)
            MQ.StaticMath(mathField).latex(latex);
        } catch (error) {
            console.error('Erro ao renderizar MathQuill:', error);
            element.innerHTML = `<span class="text-danger">Erro: ${error.message}</span>`;
        }
    },

    renderEditable: function(elementId, initialLatex, dotNetHelper) {
        const element = document.getElementById(elementId);
        if (!element) {
            console.error(`Elemento ${elementId} não encontrado`);
            return;
        }

        element.innerHTML = '';

        try {
            if (typeof MathQuill === 'undefined') {
                console.error('MathQuill não está carregado');
                return;
            }

            const MQ = MathQuill.getInterface(2);
            const mathField = MQ.MathField(element, {
                spaceBehavesLikeTab: true,
                leftRightIntoCmdGoes: 'up',
                restrictMismatchedBrackets: true,
                sumStartsWithNEquals: true,
                supSubsRequireOperand: true,
                charsThatBreakOutOfSupSub: '+-=<>',
                autoSubscriptNumerals: true,
                autoCommands: 'pi theta sqrt sum int',
                autoOperatorNames: 'sin cos tan sec csc cot log ln',
                handlers: {
                    edit: function() {
                        const latex = mathField.latex();
                        if (dotNetHelper) {
                            dotNetHelper.invokeMethodAsync('OnLatexChanged', latex);
                        }
                    }
                }
            });

            if (initialLatex) {
                mathField.latex(initialLatex);
            }

            // Armazenar referência para uso posterior
            element.mathField = mathField;

            mathField.focus();
        } catch (error) {
            console.error('Erro ao criar MathQuill editável:', error);
        }
    },

    // Obtém o LaTeX atual de um campo editável
    getLatex: function(elementId) {
        const element = document.getElementById(elementId);
        if (!element || !element.mathField) {
            return '';
        }
        return element.mathField.latex();
    },

    // Define o LaTeX de um campo editável
    setLatex: function(elementId, latex) {
        const element = document.getElementById(elementId);
        if (!element || !element.mathField) {
            return;
        }
        element.mathField.latex(latex);
    },

    // Limpa um campo editável
    clear: function(elementId) {
        const element = document.getElementById(elementId);
        if (!element || !element.mathField) {
            return;
        }
        element.mathField.latex('');
    }
};

// Inicialização quando o documento estiver pronto
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function() {
        console.log('MathQuill Helper carregado');
    });
} else {
    console.log('MathQuill Helper carregado');
}
