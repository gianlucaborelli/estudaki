<script lang="ts">
	import { onMount } from 'svelte';
	import { Button } from 'flowbite-svelte';
	import { ClipboardCheckOutline, FlagOutline } from 'flowbite-svelte-icons';
	import type { Question } from '$lib/questions/types/question';
	import { getExamCategoryLabel } from '../types/labels';

	type Props = {
		question: Question;
	};

	let { question }: Props = $props();
	let subareasValue = $state<HTMLSpanElement>();
	let canExpandSubareas = $state(false);
	let subareasExpanded = $state(false);

	function updateSubareasOverflow() {
		canExpandSubareas = (subareasValue?.scrollWidth ?? 0) > (subareasValue?.clientWidth ?? 0);
	}

	onMount(() => {
		const resizeObserver = new ResizeObserver(updateSubareasOverflow);

		if (subareasValue) {
			resizeObserver.observe(subareasValue);
			updateSubareasOverflow();
		}

		return () => resizeObserver.disconnect();
	});
</script>

<header class="question-header">
	<div class="question-identity">
		<span>Questão</span>
		<strong>{question.questionNumber}</strong>
	</div>

	<dl class="question-metadata">
		<div>
			<dt>Ano</dt>
			<dd>{question.year}</dd>
		</div>
		<div>
			<dt>Categoria</dt>
			<dd>{getExamCategoryLabel(question.examCategory)}</dd>
		</div>
		<div>
			<dt>Banca</dt>
			<dd>{question.examinerOrganization}</dd>
		</div>
		<div>
			<dt>Instituição</dt>
			<dd>{question.contractingOrganization}</dd>
		</div>
		<div>
			<dt>Edital</dt>
			<dd>{question.publicNoticeNumber}</dd>
		</div>
		<div>
			<dt>Fase</dt>
			<dd>{question.phase}</dd>
		</div>
		<div>
			<dt>Área</dt>
			<dd>{question.mainArea}</dd>
		</div>
		<div class:subareasExpanded class="subareas-metadata">
			<dt>Subáreas</dt>
			<dd>
				<span
					bind:this={subareasValue}
					class:subareas-value-expanded={subareasExpanded}
					class="subareas-value"
				>
					{question.subAreas.join(', ')}
				</span>
				{#if canExpandSubareas}
					<button
						type="button"
						aria-expanded={subareasExpanded}
						onclick={() => (subareasExpanded = !subareasExpanded)}
					>
						{subareasExpanded ? 'Mostrar menos' : 'Ver todas'}
					</button>
				{/if}
			</dd>
		</div>
	</dl>

	{#if question.positions.length > 0}
		<div class="question-context">
			<p>{question.positions.join(', ')}</p>
		</div>
	{/if}

	<div class="header-actions">
		<Button class="copy-question" aria-label="Copiar questão" title="Copiar questão">
			<ClipboardCheckOutline class="h-6 w-6 shrink-0" />
		</Button>
		<Button class="signalize-question" aria-label="Sinalizar questão" title="Sinalizar questão">
			<FlagOutline class="h-6 w-6 shrink-0" />
		</Button>
	</div>
</header>

<style>
	.question-header {
		display: grid;
		grid-template-columns: auto minmax(0, 1fr) auto;
		grid-template-areas:
			'identity metadata actions'
			'identity context actions';
		gap: 0.75rem 1.25rem;
		align-items: start;		
		border-bottom: 1px solid var(--border);
	}

	.question-identity {
		display: grid;
		grid-area: identity;
		gap: 0.125rem;
		min-width: 3.75rem;
		padding-right: 1.25rem;
		border-right: 2px solid var(--secondary);
		color: var(--secondary);
	}

	.question-identity span {
		font-size: 0.6875rem;
		font-weight: 700;
		letter-spacing: 0.08em;
		text-transform: uppercase;
	}

	.question-identity strong {
		font-size: 1.625rem;
		line-height: 1;
	}

	.question-metadata {
		display: grid;
		grid-area: metadata;
		grid-template-columns: repeat(3, minmax(0, 1fr));
		gap: 0.625rem 1.25rem;
		margin: 0;
	}

	.question-metadata div {
		min-width: 0;
	}

	.question-metadata dt {
		margin-bottom: 0.125rem;
		color: var(--text-muted);
		font-size: 0.6875rem;
		font-weight: 700;
		letter-spacing: 0.06em;
		text-transform: uppercase;
	}

	.question-metadata dd {
		overflow: hidden;
		margin: 0;
		color: var(--text);
		font-size: 0.8125rem;
		font-weight: 600;
		line-height: 1.35;
		text-overflow: ellipsis;
		white-space: nowrap;
	}

	.subareas-metadata.subareasExpanded {
		grid-column: span 2;
	}

	.subareas-metadata dd {
		overflow: visible;
	}

	.subareas-value {
		display: block;
		overflow: hidden;
		color: inherit;
		font-family: inherit;
		font-size: inherit;
		font-weight: inherit;
		line-height: inherit;
		text-overflow: ellipsis;
		white-space: nowrap;
	}

	.subareas-value-expanded {
		overflow: visible;
		white-space: normal;
	}

	.subareas-metadata button {
		margin: 0.25rem 0 0;
		padding: 0;
		border: 0;
		background: transparent;
		color: var(--tertiary);
		font: inherit;
		font-size: 0.75rem;
		font-weight: 700;
		cursor: pointer;
	}

	.subareas-metadata button:hover {
		color: var(--tertiary-hover);
		text-decoration: underline;
	}

	.subareas-metadata button:focus-visible {
		outline: 2px solid var(--tertiary);
		outline-offset: 2px;
	}

	.question-context {
		grid-area: context;
		min-width: 0;
	}

	.question-context p {
		overflow: hidden;
		margin: 0;
		color: var(--secondary);
		font-size: 0.8125rem;
		font-weight: 600;
		line-height: 1.4;
		text-overflow: ellipsis;
		white-space: nowrap;
	}

	.header-actions {
		display: flex;
		grid-area: actions;
		gap: 0.125rem;
	}

	:global(.signalize-question, .copy-question) {
		display: inline-flex;
		align-items: center;
		justify-content: center;
		width: 2.25rem;
		height: 2.25rem;
		background-color: transparent;
		color: var(--tertiary);
		cursor: pointer;
		padding: 0;
		border: none;
		outline: none;
		box-shadow: none;

		transition:
			color 0.2s ease,
			transform 0.2s ease-in-out;
	}
	:global(.signalize-question:active, .copy-question:active) {
		transform: scale(1.5);
	}

	:global(.signalize-question:hover, .copy-question:hover) {
		color: var(--tertiary-hover);
	}

	:global(.signalize-question:focus, .copy-question:focus) {
		outline: none;
		box-shadow: none;
		border: none;
	}

	:global(.signalize-question:focus-visible, .copy-question:focus-visible) {
		outline: 2px solid var(--tertiary);
		outline-offset: 2px;
	}

	@media (max-width: 800px) {
		.question-header {
			grid-template-columns: auto minmax(0, 1fr);
			grid-template-areas:
				'identity actions'
				'metadata metadata'
				'context context';
		}

		.header-actions {
			justify-self: end;
		}
	}

	@media (max-width: 520px) {
		.question-metadata {
			grid-template-columns: repeat(2, minmax(0, 1fr));
			gap: 0.75rem 1rem;
		}

		.subareas-metadata.subareasExpanded {
			grid-column: span 2;
		}
	}
</style>
