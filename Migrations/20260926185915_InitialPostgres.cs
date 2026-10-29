using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SinaisPeloMundo.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    DtNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Telefone = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Numero = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Complemento = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Bairro = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Cidade = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Email = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Senha = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Admin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Hoteis",
                columns: table => new
                {
                    ReservaHotelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeHotel = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    EnderecoHotel = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    DataCheckin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataCheckout = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hoteis", x => x.ReservaHotelId);
                });

            migrationBuilder.CreateTable(
                name: "Interpretes",
                columns: table => new
                {
                    InterpreteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeInterprete = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    DtNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Telefone = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    PrecoDiaria = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interpretes", x => x.InterpreteId);
                });

            migrationBuilder.CreateTable(
                name: "Passagens",
                columns: table => new
                {
                    PassagemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Transporte = table.Column<string>(type: "text", nullable: false),
                    TipoPassagem = table.Column<string>(type: "text", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false),
                    Poltrona = table.Column<int>(type: "integer", nullable: false),
                    PlacaTransporte = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    HorarioPartida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LocalPartida = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HorarioChegada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LocalChegada = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passagens", x => x.PassagemId);
                });

            migrationBuilder.CreateTable(
                name: "Pacotes",
                columns: table => new
                {
                    PacoteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PassagemId = table.Column<int>(type: "integer", nullable: false),
                    ReservaHotelId = table.Column<int>(type: "integer", nullable: false),
                    InterpreteId = table.Column<int>(type: "integer", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false),
                    UrlImagem = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Destino = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacotes", x => x.PacoteId);
                    table.ForeignKey(
                        name: "FK_Pacotes_Hoteis_ReservaHotelId",
                        column: x => x.ReservaHotelId,
                        principalTable: "Hoteis",
                        principalColumn: "ReservaHotelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pacotes_Interpretes_InterpreteId",
                        column: x => x.InterpreteId,
                        principalTable: "Interpretes",
                        principalColumn: "InterpreteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pacotes_Passagens_PassagemId",
                        column: x => x.PassagemId,
                        principalTable: "Passagens",
                        principalColumn: "PassagemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    PedidoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    PacoteId = table.Column<int>(type: "integer", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false),
                    FormaPagamento = table.Column<string>(type: "text", nullable: false),
                    Parcelas = table.Column<int>(type: "integer", nullable: false),
                    Cancelado = table.Column<bool>(type: "boolean", nullable: false),
                    DataEfetivacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.PedidoId);
                    table.ForeignKey(
                        name: "FK_Pedidos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedidos_Pacotes_PacoteId",
                        column: x => x.PacoteId,
                        principalTable: "Pacotes",
                        principalColumn: "PacoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Cpf",
                table: "Clientes",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interpretes_Cpf",
                table: "Interpretes",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interpretes_Email",
                table: "Interpretes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacotes_InterpreteId",
                table: "Pacotes",
                column: "InterpreteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacotes_PassagemId",
                table: "Pacotes",
                column: "PassagemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacotes_ReservaHotelId",
                table: "Pacotes",
                column: "ReservaHotelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteId",
                table: "Pedidos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_PacoteId",
                table: "Pedidos",
                column: "PacoteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Pacotes");

            migrationBuilder.DropTable(
                name: "Hoteis");

            migrationBuilder.DropTable(
                name: "Interpretes");

            migrationBuilder.DropTable(
                name: "Passagens");
        }
    }
}
