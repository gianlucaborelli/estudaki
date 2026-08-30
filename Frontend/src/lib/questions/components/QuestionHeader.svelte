<script lang="ts">
	import { Button } from 'flowbite-svelte';
	import { ClipboardCheckOutline, FlagOutline } from 'flowbite-svelte-icons';
	import type { Question } from '$lib/questions/types/question';
	import { getExamCategoryLabel } from '../types/labels';

	type Props = {
		question: Question;
	};

	let { question }: Props = $props();
</script>

<div class="question-card-head">
	<div class="public-notice-info">
		<h3 class="year">Ano: <span>{question.year}</span></h3>
		<h3 class="exam-category">
			Categoria: <span>{getExamCategoryLabel(question.examCategory)}</span>
		</h3>
		<h3 class="examiner-organization">Banca: <span>{question.examinerOrganization}</span></h3>
		<h3 class="examiner-organization">
			Instituição: <span>{question.contractingOrganization}</span>
		</h3>
		<div class="header-actions">
			<Button class="copy-question">
				<ClipboardCheckOutline class="h-6 w-6 shrink-0" />
			</Button>
			<Button class="signalize-question">
				<FlagOutline class="h-6 w-6 shrink-0" />
			</Button>
		</div>
	</div>
	<div class="exam-info">
		<h3 class="public-notice-number">Edital: <span> {question.publicNoticeNumber}</span></h3>
		<h3 class="phase">Fase: <span>{question.phase}</span></h3>
	</div>
	<div>
		<h3 class="positions">{question.positions.toString()}</h3>
	</div>
</div>

<style>
	.question-card-head {
		display: grid;
		grid-template-columns: minmax(0, 1fr) auto;
		grid-template-areas:
			'notice positions'
			'exam positions';
		gap: 0.625rem 2rem;
		align-items: start;
		padding-bottom: 2rem;
	}

	.positions {
		grid-area: positions;
		max-width: 20rem;
		margin: 0;
		color: var(--secondary);
		font-size: 0.8125rem;
		font-weight: 600;
		line-height: 1.45;
		text-align: right;
	}

	.public-notice-info,
	.exam-info {
		display: flex;
		flex-wrap: wrap;
		gap: 0.375rem 1rem;
		font-size: 0.875rem;
		align-items: center;
	}

	.public-notice-info {
		grid-area: notice;
	}

	.exam-info {
		grid-area: exam;
	}

	.public-notice-info h3,
	.exam-info h3 {
		color: var(--font-color);
		font-size: 0.8125rem;
		font-weight: 600;
		line-height: 1.4;
	}

	.public-notice-info span,
	.exam-info span {
		color: var(--secondary);
		font-size: 0.875rem;
		font-weight: 400;
	}

	.header-actions {
		display: flex;
		gap: 0.125rem;
		margin-left: auto;
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
</style>
