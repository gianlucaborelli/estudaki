<script lang="ts">
	import type { PageData } from './$types';
	import Seo from '$lib/shared/seo/seo.svelte';
	import QuestionRender from '$lib/questions/components/QuestionRender.svelte';
	import { getQuestionTypeLabel } from '$lib/questions/types/labels';

	let { data }: { data: PageData } = $props();

	const question = $derived(data.question);

	function stripHtml(html: string, maxLength?: number): string {
		let text = html
			.replace(/<[^>]*>/g, ' ')
			.replace(/\s+/g, ' ')
			.trim();

		if (maxLength && text.length > maxLength) {
			text = `${text.slice(0, maxLength)}...`;
		}

		return text;
	}

	const examName = $derived(question.examinerOrganization || question.questionType);

	const title = $derived(
		`Questão ${question.questionNumber} - ${examName} ${question.year} - ${question.mainArea} | EstudaKi`
	);

	const description = $derived.by(() => {
		let text = `Pratique questão de ${question.mainArea}`;

		if (question.subAreas.length > 0) {
			text += ` sobre ${question.subAreas.slice(0, 2).join(', ')}`;
		}

		text += ` do ${examName} ${question.year}. Resolva gratuitamente e aprimore seus conhecimentos.`;

		return text;
	});

	const keywords = $derived(
		[
			question.mainArea,
			getQuestionTypeLabel(question.questionType),
			`questão ${question.questionNumber}`,
			'questões online',
			'estudo gratuito',
			...question.subAreas.slice(0, 3),
			question.examinerOrganization,
			`${question.examinerOrganization} ${question.year}`
		]
			.filter(Boolean)
			.join(', ')
	);

	const canonicalUrl = $derived(`https://estudaki.com.br/questions/${question.questionId}`);

	const jsonLd = $derived.by(() => {
		const correctChoice = question.choices.find((choice) => choice.isCorrect);

		return JSON.stringify({
			'@context': 'https://schema.org',
			'@type': 'Question',
			name: `Questão ${question.questionNumber} - ${examName} ${question.year}`,
			text: stripHtml(question.statement, 200),
			answerCount: question.choices.length,
			eduQuestionType: question.questionType,
			educationalLevel: question.educationLevel,
			learningResourceType: 'Exam Question',
			about: {
				'@type': 'Thing',
				name: question.mainArea
			},
			acceptedAnswer: {
				'@type': 'Answer',
				text: correctChoice ? stripHtml(correctChoice.explanation, 200) : ''
			},
			author: {
				'@type': 'Organization',
				name: examName
			},
			datePublished: question.createdAt.slice(0, 10)
		});
	});
</script>

<Seo {title} {description} {keywords} {canonicalUrl} {jsonLd} />

<box gap={4}>
	<QuestionRender class="question" {question} />
</box>

<style>
	:global(.question) {
		padding: 1rem;
		margin: 1rem 20px;
		margin-bottom: 1rem;
	}
</style>
