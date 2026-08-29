<script lang="ts">
    import { resolve } from '$app/paths';	
</script>

<section class="hero">
	<div class="hero-content">
		<div >		

			<h1>
				Encontre.
				<span>Resolva.</span>
				Aprenda.
			</h1>

			<p class="hero-description">
				Um grande repositório de questões de vestibulares, concursos
				públicos e OAB. Encontre provas, disciplinas e assuntos para
				praticar exatamente o que você precisa.
			</p>

			<div class="search-box">

				<div class="search-input">

					<span>⌕</span>

					<input
						type="text"
						placeholder="Busque por questão, assunto, prova ou instituição..."
					/>

				</div>

				<button type="button" onclick={() => window.location.href = resolve('/questions')}>
					Buscar
				</button>

			</div>
		</div>

		<div class="hero-repository">
            <div class="repository-window">

                <div class="window-header">
                    <span>Repositório de questões</span>
                    <span class="result-count">1.248 questões</span>
                </div>

                <div class="repository-content">

                    <div class="question-meta">
                        <span>FGV</span>
                        <span>2025</span>
                        <span>Direito Constitucional</span>
                    </div>

                    <h3>
                        Direitos e garantias fundamentais previstos
                        na Constituição Federal.
                    </h3>

                    <div class="question-lines">
                        <div></div>
                        <div></div>
                        <div></div>
                    </div>

                </div>

                <div class="repository-footer">
                    <span>+ milhares de questões</span>

                    <a href={resolve('/questions')} class="text-link">
                        Explorar →
                    </a>
                </div>

            </div>
        </div>
	</div>
</section>

<style>
	:global(body) {
		overflow-x: hidden;     
	}

	h1 {
		margin: 0;
		color: var(--text);
		font-size: clamp(3rem, 5vw, 5rem);
		line-height: 0.98;
		letter-spacing: -0.055em;
		font-weight: 800;
	}

    .hero {
		position: relative;
		overflow: hidden;
		min-height: 680px;

		background:
			radial-gradient(
				circle at 85% 45%,
				color-mix(in srgb, var(--primary) 18%, transparent),
				transparent 35%
			),
			radial-gradient(
				circle at 15% 20%,
				color-mix(in srgb, var(--tertiary) 10%, transparent),
				transparent 30%
			),
			var(--background);
	}	

	.hero-content {
		position: relative;

		width: 100%;
		max-width: 1440px;

		margin: 0 auto;
		padding: 100px 32px;

		display: grid;
		grid-template-columns: minmax(0, 1fr) minmax(450px, 0.9fr);
		align-items: center;
		gap: 90px;
	}
	
    h1 span {
		display: flex;        
        letter-spacing: normal;
		color: var(--tertiary);
	}

	.hero-description {
		max-width: 650px;

		margin: 30px 0;

		color: var(--text-muted);

		font-size: 1.08rem;
		line-height: 1.75;
	}

	/* =========================
	   SEARCH
	========================= */

	.search-box {
		display: flex;
		align-items: stretch;

		max-width: 680px;

		padding: 6px;

		border: 1px solid var(--border);
		border-radius: 16px;

		background: var(--surface-elevated);

		box-shadow:
			0 15px 40px
			color-mix(in srgb, var(--text) 7%, transparent);

		transition:
			border-color 0.2s ease,
			box-shadow 0.2s ease;
	}

	.search-box:focus-within {
		border-color: var(--primary);

		box-shadow:
			0 0 0 4px
			color-mix(in srgb, var(--primary) 12%, transparent);
	}

	.search-input {
		flex: 1;

		display: flex;
		align-items: center;

		min-width: 0;

		padding: 0 16px;

		gap: 10px;
	}

	.search-input > span {
		flex-shrink: 0;

		color: var(--text-muted);

		font-size: 1.5rem;
		line-height: 1;
	}

	.search-input input {
		width: 100%;

		border: none;
		outline: none;

		background: transparent;

		color: var(--text);

		font-family: inherit;
		font-size: 0.9rem;
	}

	.search-input input::placeholder {
		color: var(--text-muted);
		opacity: 0.8;
	}

	.search-box button {
		padding: 13px 23px;

		border: none;
		border-radius: 11px;

		background: var(--secondary);

		color: white;

		font-family: inherit;
		font-size: 0.9rem;
		font-weight: 700;

		cursor: pointer;

		transition:
			background 0.2s ease,
			transform 0.2s ease;
	}

	.search-box button:hover {
		background: var(--secondary-hover);
		transform: translateY(-1px);
	}

	.search-box button:active {
		background: var(--primary-active);

		transform: translateY(0);
	}	

	/* =========================
	   REPOSITORY PREVIEW
	========================= */

	.hero-repository {
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .repository-window {
        width: min(100%, 420px);

        border: 1px solid var(--border);
        border-radius: 20px;

        background: var(--surface-elevated);

        box-shadow:
            0 25px 60px
            color-mix(in srgb, var(--text) 10%, transparent);

        overflow: hidden;

        transform: rotate(1deg);
    }

    .window-header {
        display: flex;
        align-items: center;
        justify-content: space-between;

        padding: 14px 18px;

        border-bottom: 1px solid var(--border);

        background: var(--surface);

        color: var(--text-muted);

        font-size: 0.7rem;
        font-weight: 600;
    }

    .result-count {
        color: var(--primary);
        font-weight: 800;
    }

    .repository-content {
        padding: 24px;
    }

    .question-meta {
        display: flex;
        flex-wrap: wrap;
        gap: 6px;

        margin-bottom: 16px;
    }

    .question-meta span {
        padding: 4px 8px;

        border-radius: 6px;

        background: var(--surface);

        color: var(--text-muted);

        font-size: 0.6rem;
        font-weight: 700;
    }

    .question-meta span:first-child {
        background: color-mix(
            in srgb,
            var(--primary) 12%,
            transparent
        );

        color: var(--primary);
    }

    .repository-content h3 {
        margin: 0;

        color: var(--text);

        font-size: 0.95rem;
        line-height: 1.55;
    }

    .question-lines {
        display: flex;
        flex-direction: column;
        gap: 8px;

        margin-top: 22px;
    }

    .question-lines div {
        height: 7px;

        border-radius: 999px;

        background: var(--border);
    }

    .question-lines div:nth-child(1) {
        width: 90%;
    }

    .question-lines div:nth-child(2) {
        width: 75%;
    }

    .question-lines div:nth-child(3) {
        width: 55%;
    }

    .repository-footer {
        display: flex;
        align-items: center;
        justify-content: space-between;

        padding: 14px 18px;

        border-top: 1px solid var(--border);

        background: var(--surface);

        color: var(--text-muted);

        font-size: 0.65rem;
    }

    .repository-footer a {
        color: var(--primary);

        font-weight: 800;
        text-decoration: none;
    }	
	
	/* =========================
	   FOOTER
	========================= */

	.repository-footer {
		display: flex;
		align-items: center;
		justify-content: space-between;

		padding: 16px 22px;

		border-top: 1px solid var(--border);

		background: var(--surface);

		color: var(--text-muted);

		font-size: 0.7rem;
	}

	.repository-footer a {
		color: var(--primary);

		font-weight: 800;

		text-decoration: none;
	}

	.repository-footer a:hover {
		color: var(--primary-hover);
	}

	.text-link {
		display: inline-flex;
		align-items: center;
		letter-spacing: 0.10em;
		color: var(--secondary) !important;
	}

	/* =========================
	   RESPONSIVE
	========================= */

	@media (max-width: 1100px) {
		.hero-content {
			grid-template-columns: 1fr;

			max-width: 850px;

			gap: 60px;

			text-align: center;
		}		

		.hero-description {
			margin-left: auto;
			margin-right: auto;
		}

		.search-box {
			margin-left: auto;
			margin-right: auto;

			text-align: left;
		}

		.hero-repository {
			min-height: 450px;
		}
	}

	@media (max-width: 600px) {
		.hero {
			min-height: auto;
		}

		.hero-content {
			padding: 70px 20px;
		}

		.hero h1 {
			font-size: clamp(3rem, 15vw, 4rem);
		}

		.hero-description {
			font-size: 1rem;
		}

		.search-box {
			flex-direction: column;

			gap: 6px;
		}

		.search-input {
			min-height: 48px;
		}

		.search-box button {
			width: 100%;
		}

		.hero-repository {
			min-height: 380px;
		}

		.repository-window {
			transform: none;
		}

		.window-header {
			padding: 15px 18px;
		}

		.repository-footer {
			flex-direction: column;
			align-items: flex-start;

			gap: 8px;
		}
	}	
	
	@media (max-width: 1000px) {
		.hero-content {
			grid-template-columns: 1fr;
			gap: 60px;
		}

		.hero-description {
			margin-left: auto;
			margin-right: auto;
		}				
	}

	@media (max-width: 640px) {
		.hero-content {
			padding-top: 70px;
			padding-bottom: 70px;
		}

		h1 {
			font-size: 3rem;
		}
	}
</style>
