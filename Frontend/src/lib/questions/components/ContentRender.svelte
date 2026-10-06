<script lang="ts">
	import 'katex/dist/katex.min.css';

	type Props = {
		content: string;
	};

	let { content }: Props = $props();
</script>

<div class="question-content">
	<!-- eslint-disable-next-line svelte/no-at-html-tags -->
	{@html content}
</div>

<style>
	/* Classes geradas pelo editor Quill usado ao cadastrar as questões.
	   Sem essas regras, atributos como alinhamento, tamanho de fonte,
	   recuo e direção de texto ficam sem efeito ao renderizar o conteúdo. */
	.question-content :global(.ql-align-center) {
		text-align: center;
	}

	.question-content :global(.ql-align-right) {
		text-align: right;
	}

	.question-content :global(.ql-align-justify) {
		text-align: justify;
	}

	.question-content :global(.ql-align-center img),
	.question-content :global(.ql-align-center video),
	.question-content :global(.ql-align-center iframe) {
		margin-right: auto;
		margin-left: auto;
	}

	.question-content :global(.ql-align-right img),
	.question-content :global(.ql-align-right video),
	.question-content :global(.ql-align-right iframe) {
		margin-right: 0;
		margin-left: auto;
	}

	.question-content :global(.ql-size-small) {
		font-size: 0.75em;
	}

	.question-content :global(.ql-size-large) {
		font-size: 1.5em;
	}

	.question-content :global(.ql-size-huge) {
		font-size: 2.5em;
	}

	.question-content :global(.ql-font-serif) {
		font-family: Georgia, 'Times New Roman', serif;
	}

	.question-content :global(.ql-font-monospace) {
		font-family: Monaco, 'Courier New', monospace;
	}

	.question-content :global(.ql-direction-rtl) {
		direction: rtl;
		text-align: inherit;
	}

	.question-content :global(.ql-indent-1) {
		padding-left: 3em;
	}

	.question-content :global(.ql-indent-2) {
		padding-left: 6em;
	}

	.question-content :global(.ql-indent-3) {
		padding-left: 9em;
	}

	.question-content :global(.ql-indent-4) {
		padding-left: 12em;
	}

	.question-content :global(.ql-indent-5) {
		padding-left: 15em;
	}

	.question-content :global(.ql-indent-6) {
		padding-left: 18em;
	}

	.question-content :global(.ql-indent-7) {
		padding-left: 21em;
	}

	.question-content :global(.ql-indent-8) {
		padding-left: 24em;
	}

	.question-content :global(.ql-formula) {
		padding: 0 0.15rem;
	}
	
	.question-content :global(.katex) {
		font-size: 1em !important;
	}
</style>
