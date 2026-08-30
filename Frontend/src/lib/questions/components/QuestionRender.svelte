<script lang="ts">
	import type { Question } from '$lib/questions/types/question';
	import { Button } from 'flowbite-svelte';
	import { ClipboardCheckOutline, FlagOutline } from 'flowbite-svelte-icons';
	import QuestionContentRender from './QuestionContentRender.svelte';

	type Props = {
		question: Question;
		class?: string;
	};

	let { question, class: className = '' }: Props = $props();
</script>

<div class={`question-card ${className}`}>
	<div class="question-card-head">
		<div class="public-notice-info">
			<h3 class="year">Ano: <span>{question.year}</span></h3>
			<h3 class="exam-category">Categoria: <span>{question.examCategory}</span></h3>
			<h3 class="examiner-organization">Banca: <span>{question.examinerOrganization}</span></h3>
			<h3 class="examiner-organization">
				Organizadora: <span>{question.contractingOrganization}</span>
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

	<div class="content">
		<div class="question-support">
			{#each question.questionSupports as support (support.id)}
				<QuestionContentRender content={support.contents} />
			{/each}
		</div>

		<div class="statement">
			<QuestionContentRender content={question.questionContents} />
		</div>
	</div>

	<div class="choices">	
		{#each question.choices as choice (choice.option)}
			<div class="choice">			
				<QuestionContentRender content={choice.contentBlocks} />
			</div>
		{/each}
	</div>
</div>

<style>
	.question-card {
		margin-top: 1.5rem;
		border-top: 1px solid var(--border);
		padding-top: 1rem;
	}

	.positions {
		grid-area: positions;
	}

	.public-notice-info,
	.exam-info {
		display: flex;
		font-size: 0.875rem;
		align-items: center;
		margin-bottom: 0.5rem;
	}

	.public-notice-info h3,
	.exam-info h3 {
		color: var(--font-color);
		font-weight: 500;
	}

	.public-notice-info span,
	.exam-info span {
		color: var(--secondary);
		size: 0.875rem;
		font-weight: 400;
	}

	.header-actions {
		display: flex;
		size: 0.875rem;
	}

	:global(.signalize-question, .copy-question) {
		background-color: transparent;
		color: var(--tertiary);
		cursor: pointer;
		padding: 0px 10px;
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
		outline: none;
		box-shadow: none;
		border: none;
	}

	span {
		font-weight: bold;
		font-size: 0.875rem;
		margin-right: 0.25rem;
	}
</style>
