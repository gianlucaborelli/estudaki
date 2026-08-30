<script lang="ts">
	import sanitizeHtml from 'sanitize-html';
	import type { BlockContent } from '$lib/questions/types/question';

	type Props = {
		content: BlockContent[];
		class?: string;
	};

	let { content, class: className = '' }: Props = $props();

	function sanitizeQuestionContent(html: string): string {
		const withBreaks = html.replace(/\r?\n/g, '<br>');

		return sanitizeHtml(withBreaks, {
			allowedTags: ['strong', 'em', 'i', 'u', 'br'],
			allowedAttributes: {}
		});
	}
</script>

<div class={`question-content ${className}`}>
	{#each content as content (content.order)}
		{#if content.title}
			<h3>{content.title}</h3>
		{/if}

		{#if content.type === 'image' && content.key}
			<img class="question-content-image" src={content.key} alt={content.description} />
		{:else if content.type === 'paragraph' && content.text}
			<div class="question-content-text">
				<!-- eslint-disable-next-line svelte/no-at-html-tags -->
				{@html sanitizeQuestionContent(content.text)}
			</div>
		{/if}

		{#if content.source}
			<small class="question-content-source">
				{content.source}
			</small>
		{/if}
	{/each}
</div>

<style>
	.question-content-text {
		margin-bottom: 0.5rem;
		text-align: justify;
	}

	.question-content-image {
		display: block;
		max-width: 100%;
		height: auto;
		margin: 0 auto 0.5rem;
	}

	.question-content-source {
		display: block;
		margin-bottom: 0.5rem;
		text-align: right;
		font-size: 0.875rem;
		color: var(--tertiary);
	}
</style>
