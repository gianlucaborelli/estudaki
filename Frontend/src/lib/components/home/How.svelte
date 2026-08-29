<script lang="ts">
	import { Button} from 'flowbite-svelte';    
	import {
		ArrowRightOutline
	} from 'flowbite-svelte-icons';
</script>

<section class="section how-section">
	<div class="how-content">
		<div class="how-copy">
			<span class="eyebrow">COMO FUNCIONA</span>

			<h2>
				Seu próximo passo
				<span>começa com uma questão.</span>
			</h2>

			<p>
				O EstudaKi foi criado para tornar a prática mais simples. Escolha o
				que deseja estudar, encontre questões relevantes e comece a testar
				seus conhecimentos.
			</p>

			<Button href="/question" class="text-link">
				Explorar questões
				<ArrowRightOutline />
			</Button>
		</div>

		<div class="steps">
			<div class="step">
				<div class="step-number">01</div>

				<div>
					<h3>Escolha o que estudar</h3>
					<p>
						Filtre por área, disciplina, instituição, prova ou assunto.
					</p>
				</div>
			</div>

			<div class="step">
				<div class="step-number">02</div>

				<div>
					<h3>Resolva questões</h3>
					<p>
						Teste seus conhecimentos com questões reais de diferentes provas.
					</p>
				</div>
			</div>

			<div class="step">
				<div class="step-number">03</div>

				<div>
					<h3>Aprenda com seus erros</h3>
					<p>
						Revise suas respostas e identifique onde precisa melhorar.
					</p>
				</div>
			</div>
		</div>
	</div>
</section>

<style>
	:global(body) {
		overflow-x: hidden;        
		width: 100%;
	}		

	.section {
		max-width: 1440px;
		padding: 25px 24px;
	}	
	
	.how-copy h2 {
		margin: 0;
		color: var(--text);
		font-size: clamp(2.2rem, 4vw, 3.6rem);
		line-height: 1.05;
		letter-spacing: -0.04em;
		font-weight: 800;
	}
	
	.how-copy h2 span
	{
		color: var(--primary);
	}		

	.how-section {
		background: var(--surface);
	}

	.how-content {
		max-width: 1440px;
		margin: 0 auto;
		display: grid;
		grid-template-columns: 0.9fr 1.1fr;
		gap: 100px;
		align-items: center;
	}

	.how-copy p {
		margin: 25px 0;
		color: var(--text-muted);
		line-height: 1.8;
	}

	.steps {
		display: flex;
		flex-direction: column;
		gap: 18px;
	}

	.step {
		display: grid;
		grid-template-columns: 75px 1fr;
		gap: 20px;
		padding: 28px;
		border: 1px solid var(--border);
		border-radius: 18px;
		background: var(--surface-elevated);
	}

	.step-number {
		color: var(--primary);
		font-size: 1.4rem;
		font-weight: 800;
	}

	.step h3 {
		margin: 0 0 8px;
		color: var(--text);
		font-size: 1.1rem;
	}

	.step p {
		margin: 0;
		color: var(--text-muted);
		font-size: 0.9rem;
		line-height: 1.6;
	}	

	@media (max-width: 1000px) {
		.how-content {
			grid-template-columns: 1fr;
			gap: 60px;
		}
	}

</style>
