using UnityEngine;
using System.Collections.Generic;
using Bounds.Persistencia;
using Bounds.Cofres;
using Ging1991.Persistencia.Direcciones;
using UnityEngine.SceneManagement;
using Ging1991.Core;
using Bounds.Musica;
using Ging1991.Musica;
using Bounds.Visuales;
using Bounds.Modulos.Cartas.Persistencia.Datos;
using Ging1991.Core.Interfaces;
using Bounds.Modulos.Cartas.Persistencia;
using Bounds.Entrenamiento;
using Bounds.Cartas;
using Bounds.Persistencia.proveedores;
using Ging1991.Persistencia.Lectores;
using Bounds.Sistema;
using Bounds.Sistema.Parametros;
using Bounds.Sistema.Ilustradores;

namespace Bounds.Tienda {

	public class ControlTiendaAbrir : SingletonMonoBehaviour<ControlTiendaAbrir> {

		public GameObject objSobre;
		public List<GameObject> sobres = new List<GameObject>();
		public Cofre cofre;
		public IlustradorDeCartas ilustrador;

		public ControlParametros parametrosControl;
		public DireccionRecursos carpetaColecciones;
		public GestorDeSobres gestorDeSobres;
		public GestorDeSonidos gestorDeSonidos;
		public GestorEfectosVisuales gestorEfectosVisuales;
		public IProveedor<int, CartaBD> proveedorCartas;
		public ControlUIBounds personalizarUI;
		public CartaGenerador cartaGenerador;


		private void InicializarMusica(Direccion direccion) {
			MusicaAmbiental musicaAmbiental = MusicaAmbiental.Instancia;
			if (musicaAmbiental.actual != "GENERAL") {
				musicaAmbiental.Inicializar(new ProveedorAudios(direccion));
				musicaAmbiental.Reproducir("GENERAL");
			}
		}


		void Start() {
			parametrosControl.Inicializar();
			ParametrosGlobales parametros = parametrosControl.parametros;
			if (!RegistroGlobal.Instancia.inicializado)
				RegistroGlobal.Instancia.Inicializar(parametros);
			InicializarMusica(parametros.direcciones["MUSICA_AMBIENTAL"]);

			personalizarUI.Personalizar(parametros.direccionesGeneradas["SISTEMA"], parametros.direccionesGeneradas["COLORES"]);

			carpetaColecciones = new(parametros.direccionesGeneradas["COLECCIONES"]);
			gestorDeSobres = new(parametros.direccionesGeneradas["SOBRES"]);
			gestorDeSonidos.Inicializar(new DireccionRecursos(parametros.direccionesGeneradas["SONIDOS"]));
			gestorEfectosVisuales.Inicializar(gestorDeSonidos);
			proveedorCartas = new LectorCartas(new DireccionRecursos(parametrosControl.parametros.direccionesGeneradas["CARTAS_DATOS"]));

			cofre = new(parametros.direccionesGeneradas["COFRE"], parametros.direccionesGeneradas["COFRE_RECURSOS"]);
			ilustrador = new IlustradorDeCartas(
				new DireccionRecursos(parametrosControl.parametros.direccionesGeneradas["CARTAS_RECURSO"]),
				new DireccionDinamica(parametrosControl.parametros.direccionesGeneradas["CARTAS_DINAMICA"])
			);
			List<string> claves = new List<string>(){
				"COMPLETA100", "COMPLETA200", "COMPLETA300", "COMPLETA400", "COMPLETA500",
				"COMPLETA600", "ENERO2026", "ANTIGUOS", "BASICOS", "EQUIPOS",
				"HECHIZOS", "TRAMPAS", "AURAS", "EXPLOSION", "OCEANO",
				"OSCURIDAD", "BOSQUE", "TRUENO", "DIVINIDAD", "FAMILIA", "META", "PRINCIPIANTE"
			};

			cartaGenerador.Inicializar(
				ilustrador,
				proveedorCartas,
				new ProveedorColores(
					parametrosControl.parametros.direccionesGeneradas["COLORES"],
					TipoLector.RECURSOS
				)
			);

			foreach (string clave in claves) {
				CrearSobre(new Coleccion(clave, carpetaColecciones.Generar(clave)), ilustrador);
			}
			Organizar();
		}


		private void CrearSobre(Coleccion coleccion, IlustradorDeCartas ilustrador) {
			int cantidad = gestorDeSobres.GetCantidad(coleccion.codigo);
			if (cantidad == 0)
				return;

			GameObject instancia = Instantiate(objSobre, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			instancia.name = "Sobre " + coleccion.codigo;
			GameObject contenedor = GameObject.Find("Sobres");
			instancia.transform.SetParent(contenedor.transform);
			instancia.transform.localScale = new Vector3(1, 1, 1);
			instancia.transform.localPosition = new Vector3(0, 0, 0);
			SobreAbrir componente = instancia.GetComponent<SobreAbrir>();
			componente.Inicializar(cartaGenerador, coleccion);
			this.sobres.Add(instancia);
		}


		private void Organizar() {
			int x = 0;
			int y = 0;
			int salto = 200;
			foreach (GameObject sobre in sobres) {
				sobre.transform.localPosition = new Vector3(x, y, 0);
				x += salto;
			}

			if (sobres.Count > 0) {
				sobres[0].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
			}

		}


		public void Remover(GameObject sobre) {
			sobres.Remove(sobre);
			Organizar();
		}


		public void AgregarCarta(int cartaID, string imagen, string rareza) {
			cofre.AgregarCarta(new CartaCofreBD($"{cartaID}_{imagen}_{rareza}_1"));
		}


		public void Guardar() {
			cofre.Guardar();
		}


		public void PresionarComprar() {
			SceneManager.LoadScene("TIENDA COMPRAR");
		}

	}

}