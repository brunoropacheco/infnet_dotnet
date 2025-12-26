# infnet_dotnet
Repositório para o trabalho de arquitetura .NET do curso de pós graduação do INFNET

## 1. Modelo de Negócio:

  A Startup oferece uma plataforma SaaS para criação, distribuição e processamento de pesquisas públicas em larga escala. O produto permite que um **Gestor** configure uma **Pesquisa**, defina **Perguntas**, status e regras de participação.

  O **Gestor** precisa ser criado antes da pesquisa. 

  Após a publicação da Pesquisa, o conteúdo é distribuído por canais (redes sociais, parceiros) para alcançar **Eleitores**. Quando um Eleitor participa acessa o sistema, ele pode escolher em qual pesquisa ele vai participar. Ao escolher, ele é apresentado às pergutnas daquela pesquisa que inquire o eleitor sobre alguns dados seus e sobre as suas opções de votos.

  O eleitor responde às perguntas e registra suas **Respostas**. As **Respostas** contem as escolhas do eleitor. Como é um sistema de votação em eleições, sempre haverá perguntas sobre as opções de voto do eleitor, perguntando se votará em algum candidato, se irá a eleição, ou votará branco ou nulo, etc.

  As respostas registradas são então submetidas ao processo de **Apuração**, que valida as entradas, aplica regras de negócio (por exemplo, checagem de duplicidade) e consolida os resultados.

  Os **Resultados Sumarizados** são gerados ao final da pesquisa quando ela for encerrada (isso precisa ser acionado) e precisamos dizer quais usuarios terao acesso a este resultado.

  Para alinhar o time e o produto ao negócio, adotamos DDD (Domain Driven Design) e uma linguagem ubíqua com termos como Pesquisa, Cenário, Gestor, Eleitor, Voto, Recebimento, Apuração e Resultado Sumarizado.

## 2. Glossário de Termos

- **Pesquisa**: Agrupa uma sondagem (ex.: Eleição Municipal 2025) com título, período e opções de escolha.
- **Gestor**: Pessoa ou cliente que cria, publica e acompanha o desempenho da pesquisa.
- **Eleitor**: Participante que responde à pesquisa pela interface pública.
- **Perguntas**: Perguntas que constarão nas pesquisas. Elas serão de multipla escolha e serão textos.
- **Respostas**: Registro das escolhas do eleitor em uma pesquisa específica.
- **Apuração**: Processo que valida e consolida as respostas do eleitor para gerar os resultados oficiais.
- **Resultado Sumarizado**: Entidade que armazena o consolidado dos votos ao fim da pesquisa

## 3. Desafio

Nosso objetivo é entregar um sistema desenvolvido com ASP.NET Core 9, seguindo os princípios do Domain-Driven Design (DDD). Ele oferece uma API RESTful para realizar operações CRUD (Create, Read, Update, Delete) para as pesquisas.

## 4. Diagrama de Classes

Após a modelagem realizada, este é o uml com as principais classes do sistema:

![Diagrama de Classes](./docs/diagrams/diagrama_class.png)

## 5. Arquitetura em Camadas

### 1. PesqMgm.Domain (Camada de Domínio)
Coração da aplicação. Contém a lógica de negócios, entidades, Value Objects, agregados e interfaces de repositório.
Independente de qualquer tecnologia de infraestrutura ou UI. Não conhece banco de dados, frameworks web, etc.
Foco: Modelar o problema de negócio de forma rica e expressiva.

### 2. PesqMgm.Infrastructure (Camada de Infraestrutura)
Responsável pela persistência de dados e outras preocupações técnicas.
Implementa as interfaces de repositório definidas na camada de Domínio.
Utiliza Entity Framework Core para interagir com o banco de dados (SQL Server LocalDB).
Contém configurações de mapeamento de entidades para o banco de dados.
Add-Migration Initial -Context PesquisasDbContext -Project PesqMgm.Infrastructure.Data -StartupProject PesqMgm.Api

Dentro de PesquisaDbContext, no mapeamento para os bancos de dados, foi necessário mapear ResultadoSumarizado como uma entidade para que fosse possivel associar os usuarios leitores a este resultado, apesar de ele ser um objeto de valor.

### 3. PesqMgm.API (Camada de Apresentação/Aplicação)

Ponto de entrada da aplicação. Expõe a funcionalidade de negócio através de uma API RESTful.
Contém controladores (Controllers) para Pesquisa e Usuario que recebem requisições HTTP, orquestram as operações de domínio e retornam respostas HTTP. NO nosso caso, este projeto/camada está fazendo os papeis de apresentação e aplicação. Em teoria, a API seria só a camada de apresentação e teria as rotas das requisicoes, mas pela simplicidade da aplicação, fizemos esta otimização para diminuir código, custos e recursos computacionais.

Utiliza DTOs (Data Transfer Objects) para desacoplar a API do modelo de domínio.
Configura a injeção de dependência e o pipeline da aplicação (middleware) no arquivo Program.cs.

Esta camada precisa enxergar todas as camadas da aplicacao (Dominio e Repository neste caso) para fazer a orquestração.

### 4. PesqMgm.Infrastructure.Tests (Camada de Testes de Infraestrutura)
Contém testes unitários para a implementação do repositório, garantindo que a persistência de dados funcione corretamente.

### 5. PesqMgm.Domain.Tests (Camada de Testes de Domínio)
Contém testes unitários para as entidades e Value Objects do domínio, garantindo que a lógica de negócio esteja correta e robusta.

## 6. Padrões de Projeto Utilizados
Domain-Driven Design (DDD): Foco na modelagem do domínio de negócio, com linguagem ubíqua e conceitos de Aggregate Roots, Value Objects e Repositories. Pode ser visto no projeto de domínio existente na solução.
Repository Pattern: Abstrai a lógica de persistência de dados, permitindo que a camada de domínio trabalhe com coleções de objetos sem se preocupar com os detalhes do armazenamento.
Factory Pattern: Utilizado nos métodos Create dos Value Objects e Aggregate Roots para encapsular a lógica de criação e validação, garantindo que os objetos sejam sempre criados em um estado válido.
Value Object Pattern: Objetos que representam um conceito descritivo no domínio, definidos pela sua composição de atributos e comparados por valor, não por identidade. São imutáveis.
Aggregate Root Pattern: Entidades que são a raiz de um cluster de objetos (Aggregate), garantindo a consistência transacional dentro do agregado. Todas as operações externas devem passar pela Aggregate Root.
Dependency Injection (DI): Utilizado para gerenciar as dependências entre as camadas e componentes, promovendo o baixo acoplamento e a testabilidade.
Fluent API (EF Core): Usada para configurar o mapeamento objeto-relacional no Entity Framework Core, permitindo mapear Value Objects complexos para o banco de dados.
RESTful API: A camada de API segue os princípios REST para comunicação entre cliente e servidor, utilizando verbos HTTP e URLs semânticas.
