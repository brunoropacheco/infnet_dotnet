# infnet_dotnet
Repositório para o trabalho de arquitetura .NET do curso de pós graduação do INFNET

1. Modelo de Negócio
A Startup oferece uma plataforma SaaS para criação, distribuição e processamento de pesquisas públicas em larga escala. O produto permite que um **Gestor** configure uma **Pesquisa**, defina **Candidatos**, período de coleta e regras de participação.

Após a publicação da Pesquisa, o conteúdo é distribuído por canais (redes sociais, parceiros) para alcançar **Eleitores**. Quando um Eleitor participa acessa o sistema, ele pode escolher em qual pesquisa ele vai participar. Ao escolher, ele vota e seu **Voto** é imediatamente registrado. 

Os votos registrados são então submetidos ao processo de **Apuração**, que valida as entradas, aplica regras de negócio (por exemplo, checagem de duplicidade) e consolida os resultados.

Os **Resultados Sumarizados** (totais e porcentagens) são disponibilizados em painéis para Gestores e, conforme a configuração, ao público. Escala, disponibilidade e integridade dos dados são requisitos-chave: o serviço precisa manter a experiência do Eleitor e a confiança do Gestor mesmo durante picos de tráfego.

Para alinhar o time e o produto ao negócio, adotamos DDD (Domain Driven Design) e uma linguagem ubíqua com termos como Pesquisa, Cenário, Gestor, Eleitor, Voto, Recebimento, Apuração e Resultado Sumarizado.

2. Glossário de Termos

- **Pesquisa**: Agrupa uma sondagem (ex.: Eleição Municipal 2025) com título, período e opções de escolha.
- **Gestor**: Pessoa ou cliente que cria, publica e acompanha o desempenho da pesquisa.
- **Eleitor**: Participante que responde à pesquisa pela interface pública.
- **Candidato**: Alternativa selecionável na pesquisa (ex.: Candidato A).
- **Voto**: Registro da escolha do eleitor em um cenário específico.
- **Apuração**: Processo que valida e consolida votos para gerar os resultados oficiais.
- **Resultado Sumarizado**: Visão agregada (totais e porcentagens) disponível para gestores e público.

3. Contextos principais do negócio — Versão Compacta

- **Gestão de Pesquisas** 🔧  
  Responsabilidade: criar, configurar e publicar pesquisas e cenários; inclui ações básicas de divulgação (links e campanhas).
  Valor: autonomia ao Gestor para lançar e controlar campanhas rapidamente.

- **Operação de Votos** 🟢  
  Responsabilidade: receber votos, confirmar ao eleitor, validar entradas e consolidar resultados (coleta + apuração).
  Valor: garante experiência confiável ao eleitor e credibilidade dos resultados.

- **Resultados & Relatórios** 📊  
  Responsabilidade: painéis, resumos e insights claros (totais, percentuais, filtros por cenário).
  Valor: facilita decisões rápidas e transparência para gestores e público.



