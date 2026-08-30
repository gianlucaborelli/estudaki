<script lang="ts">
	import type { Question } from '$lib/questions/types/question';
	
	import QuestionContentRender from './QuestionContentRender.svelte';
	
	import QuestionHeader from './QuestionHeader.svelte';

	type Props = {
		question: Question;
		class?: string;
	};

	let { question, class: className = '' }: Props = $props();
	let selectedChoices = $state<string[]>([]);
</script>

<div class={`question-card ${className}`}>
	<div class="question-card-head">
		<QuestionHeader {question} />
	</div>

	<div class="content">
		{#if question.questionSupports.length > 0}
			<div class="question-support">
				{#each question.questionSupports as support (support.id)}
					<QuestionContentRender content={support.contents} />
				{/each}
			</div>
		{/if}
		<div class="statement">
			<QuestionContentRender content={question.questionContents} />
		</div>
	</div>

	<fieldset class="choices">
		{#each question.choices as choice (choice.option)}
			<label class:choice-selected={selectedChoices.includes(choice.option)} class="choice">
				<input type="checkbox" value={choice.option} bind:group={selectedChoices} />
				<span class="choice-option" aria-hidden="true">{choice.option}</span>
				<QuestionContentRender content={choice.contentBlocks} />
			</label>
		{/each}
	</fieldset>
</div>

<style>
	.question-card {
		margin-top: 1.5rem;
		border-top: 1px solid var(--border);
		padding-top: 1.5rem;
	}

	.question-card-head{
		padding-bottom: 1.5rem;
	}
	
	.question-support {
		margin-bottom: 1.25rem;
		padding: 0 3rem;
		color: var(--text);
		font-size: 0.9375rem;
	}

	.statement {
		color: var(--text);
		font-size: 1rem;
		line-height: 1.7;
	}

	.choices {
		display: grid;
		gap: 0.5rem;
		min-width: 0;
		margin: 0;
		padding: 1.25rem 0 0;
		border: 0;
	}	

	.choice {
		display: flex;
		gap: 0.875rem;
		align-items: flex-start;
		padding: 0.875rem 1rem;
		border: 1px solid var(--border);
		border-radius: 0.375rem;
		color: var(--text);
		cursor: pointer;
		line-height: 1.55;
		line-height: 1.7;
		font-size: 0.85rem;
		transition:
			border-color 0.18s ease,
			background-color 0.18s ease;
	}

	.choice:hover {
		border-color: var(--tertiary);
	}

	.choice-selected {
		border-color: var(--tertiary);
		background-color: color-mix(in srgb, var(--tertiary) 9%, transparent);
	}

	.choice input {
		position: absolute;
		opacity: 0;
		width: 1px;
		height: 1px;
		pointer-events: none;
	}

	.choice-option {
		display: grid;
		flex: 0 0 1.5rem;
		width: 1.5rem;
		height: 1.5rem;
		margin-top: 0.05rem;
		border: 1px solid var(--tertiary);
		border-radius: 0.25rem;
		color: var(--tertiary);
		font-size: 0.75rem;
		font-weight: 700;
		line-height: 1;
		place-items: center;
	}

	.choice-selected .choice-option {
		background-color: var(--tertiary);
		color: var(--text);
	}

	.choice:has(input:focus-visible) {
		outline: 2px solid var(--tertiary);
		outline-offset: 2px;
	}

	.choice :global(.question-content) {
		min-width: 0;
		flex: 1;
	}

	@media (max-width: 640px) {
		.question-card-head {
			grid-template-columns: 1fr;
			grid-template-areas:
				'notice'
				'exam'
				'positions';
			gap: 0.625rem;
		}

		.positions {
			max-width: none;
			text-align: left;
		}

		.header-actions {
			margin-left: 0;
		}
	}
</style>
