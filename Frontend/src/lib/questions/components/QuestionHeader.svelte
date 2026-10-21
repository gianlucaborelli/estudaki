<script lang="ts">
	import { onMount } from 'svelte';
	import { fly } from 'svelte/transition';
	import { Button, Dropdown, DropdownItem, Toast, ToastContainer } from 'flowbite-svelte';
	import { ClipboardCheckOutline, DownloadOutline, FlagOutline } from 'flowbite-svelte-icons';
	import type { Question } from '$lib/questions/types/question';
	import { getExamCategoryLabel } from '../types/labels';
	import QuestionIssueModal from './QuestionIssueModal.svelte';

	type Props = {
		question: Question;
	};
	let showCopyToast = $state(false);

	let { question }: Props = $props();
	let subareasValue = $state<HTMLSpanElement>();
	let canExpandSubareas = $state(false);
	let subareasExpanded = $state(false);
	let downloadDropdownOpen = $state(false);
	let issueModalOpen = $state(false);
	const downloadTriggerId = $derived(`download-question-trigger-${question.questionId}`);

	function downloadFile(url: string) {
		downloadDropdownOpen = false;
		window.open(url, '_blank', 'noopener,noreferrer');
	}

	function downloadQuestionBooklet() {
		downloadFile(question.examBookletUrl);
	}

	function downloadAnswerKey() {
		downloadFile(question.answerKeyUrl);
	}

	function htmlToPlainText(html: string): string {
		const document = new DOMParser().parseFromString(html, 'text/html');

		return document.body.textContent?.trim() ?? '';
	}

	function buildQuestionText(question: Question): string {
		const header = `(${question.examinerOrganization} - ${question.contractingOrganization} - ${question.year})`;

		const statement = htmlToPlainText(question.statement);

		const choices = question.choices
			.map((choice, index) => {
				const letter = String.fromCharCode(65 + index);
				const text = htmlToPlainText(choice.explanation);

				return `${letter}) ${text}`;
			})
			.join('\n');

		return [header, statement, choices].filter(Boolean).join('\n\n');
	}

	async function copyQuestion() {
		const text = buildQuestionText(question);

		try {
			await navigator.clipboard.writeText(text);

			showCopyToast = true;

			setTimeout(() => {
				showCopyToast = false;
			}, 2500);
		} catch (error) {
			console.error('Não foi possível copiar a questão:', error);
		}
	}

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
		<div class="header-actions-primary">
			<Button
				class="action-button copy-question"
				aria-label="Copiar questão"
				title="Copiar questão"
				onclick={copyQuestion}
			>
				<ClipboardCheckOutline class="h-5 w-5 shrink-0" />
				<span class="action-label action-label-full">Copiar questão</span>
				<span class="action-label action-label-short">Copiar</span>
			</Button>
			<Button
				id={downloadTriggerId}
				class="action-button download-question"
				aria-label="Documentos da questão"
				title="Documentos da questão"
			>
				<DownloadOutline class="h-5 w-5 shrink-0" />
				<span class="action-label action-label-full">Documentos</span>
			</Button>
			<Dropdown
				simple
				bind:isOpen={downloadDropdownOpen}
				triggeredBy="#{downloadTriggerId}"
				class="download-dropdown"
			>
				<DropdownItem disabled={!question.examBookletUrl} onclick={downloadQuestionBooklet}>
					Caderno de questões
				</DropdownItem>
				<DropdownItem disabled={!question.answerKeyUrl} onclick={downloadAnswerKey}>
					Gabarito
				</DropdownItem>
			</Dropdown>
		</div>

		<div class="header-actions-secondary">
			<Button
				class="action-button signalize-question"
				aria-label="Sinalizar questão"
				title="Sinalizar questão"
				onclick={() => (issueModalOpen = true)}
			>
				<FlagOutline class="h-5 w-5 shrink-0" />
				<span class="action-label action-label-full">Sinalizar</span>
			</Button>
		</div>
	</div>
</header>

{#if showCopyToast}
	<ToastContainer position="top-right" >
		<Toast dismissable={true} transition={fly} params={{ x: 200 }}>
			{#snippet icon()}
				<ClipboardCheckOutline class="h-6 w-6" />
			{/snippet}
			Questão copiada para a área de transferência.
		</Toast>
	</ToastContainer>
{/if}

<QuestionIssueModal questionId={question.questionId} bind:open={issueModalOpen} />

<style>
	.question-header {
		display: grid;
		grid-template-columns: auto minmax(0, 1fr);
		grid-template-areas:
			'identity metadata'
			'identity context'
			'actions actions';
		gap: 0.75rem 1.25rem;
		align-items: start;
		padding-bottom: 1.25rem;
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
		align-items: center;
		justify-content: space-between;		
		flex-wrap: nowrap;
	}

	.header-actions-primary {
		display: flex;
		align-items: center;
		gap: 0.5rem;
		min-width: 0;
	}

	.header-actions-secondary {
		display: flex;
		align-items: center;
	}

	:global(.action-button) {
		display: inline-flex;
		align-items: center;
		justify-content: center;
		gap: 0.5rem;
		min-width: 2rem;
		min-height: 2rem;
		border: 1px solid transparent;
		border-radius: 0.5rem;
		background-color: transparent;
		color: var(--tertiary);
		cursor: pointer;
		padding: 0 0.875rem;
		outline: none;
		box-shadow: none;
		font-size: 0.8125rem;
		font-weight: 700;
		white-space: nowrap;

		transition:
			color 0.2s ease,
			background-color 0.2s ease,
			border-color 0.2s ease,
			transform 0.15s ease-in-out;
	}

	:global(.action-button:active) {
		transform: scale(0.96);
	}

	:global(.action-button:hover) {
		border-color: var(--border);
		background-color: var(--surface-hover, rgba(0, 0, 0, 0.04));
		color: var(--tertiary-hover);
	}

	:global(.action-button:focus) {
		outline: none;
		box-shadow: none;
		border-color: transparent;
	}

	:global(.action-button:focus-visible) {
		outline: 2px solid var(--tertiary);
		outline-offset: 2px;
	}

	.action-label-short {
		display: none;
	}

	:global(.download-dropdown) {
		z-index: 20;
		min-width: 11rem;
		border: 1px solid var(--border);
		border-radius: 0.5rem;
		background-color: var(--surface, #fff);
		box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
	}

	:global(.download-dropdown li > button) {
		color: var(--text);
		font-size: 0.8125rem;
		font-weight: 600;
	}

	:global(.download-dropdown li > button:hover) {
		background-color: var(--surface-hover, rgba(0, 0, 0, 0.04));
		color: var(--tertiary-hover);
	}

	@media (max-width: 800px) {
		.question-header {
			grid-template-columns: minmax(0, 1fr);
			grid-template-areas:
				'identity'
				'metadata'
				'context'
				'actions';
		}

		.question-identity {
			padding-right: 0;
			padding-bottom: 0.5rem;
			border-right: none;
			border-bottom: 2px solid var(--secondary);
		}
	}

	@media (max-width: 640px) {
		.action-label-full {
			display: none;
		}

		.action-label-short {
			display: inline;
		}

		:global(.action-button) {
			padding: 0 0.625rem;
		}
	}

	@media (max-width: 360px) {
		.action-label-short {
			display: none;
		}

		:global(.copy-question) {
			min-width: 2.75rem;
			padding: 0;
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

