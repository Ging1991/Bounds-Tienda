using Bounds.Cartas;
using Bounds.Cofres;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Persistencia;
using Bounds.Modulos.Cartas.Persistencia.Datos;
using Bounds.Musica;
using Bounds.Persistencia;
using Bounds.Persistencia.Parametros;
using Bounds.Persistencia.proveedores;
using Ging1991.Core;
using Ging1991.Core.Interfaces;
using Ging1991.Musica;
using Ging1991.Persistencia.Direcciones;
using Ging1991.Persistencia.Lectores;
using Ging1991.Ventanas;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bounds.Tienda {

	public class ControlTiendaComprar : SingletonMonoBehaviour<ControlTiendaComprar> {

		public Configuracion configuracion;
		public ParametrosControl parametrosControl;
		public DireccionRecursos carpetaColecciones;
		public GestorDeSobres gestorDeSobres;
		public string escenaAnterior;
		public IProveedor<string, Sprite> selectorImagenes;
		public Cofre cofre;
		public GestorDeSonidos gestorDeSonidos;
		public ControlUIBounds personalizarUI;
		public VentanaControl ventanaControl;
		public CartaGenerador cartaGenerador;
		public RegistroGlobal registroGlobal;

		private void InicializarMusica(string direccion) {
			MusicaAmbiental musicaAmbiental = MusicaAmbiental.Instancia;
			if (musicaAmbiental.actual != "GENERAL") {
				musicaAmbiental.Inicializar(new ProveedorAudios(new DireccionRecursos(direccion)));
				musicaAmbiental.Reproducir("GENERAL");
			}
		}


		void Start() {
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;
			personalizarUI.Personalizar(parametros.direcciones["SISTEMA"], parametros.direcciones["COLORES"]);

			registroGlobal = RegistroGlobal.Instancia;
			if (!registroGlobal.inicializado) {
				registroGlobal.Inicializar(parametros.direcciones["BILLETERA"]);
			}

			configuracion = new(parametros.direcciones["CONFIGURACION"]);
			cofre = new(parametros.direcciones["COFRE"], parametros.direcciones["COFRE_RECURSOS"]);
			carpetaColecciones = new(parametros.direcciones["COLECCIONES"]);
			gestorDeSobres = new(parametros.direcciones["SOBRES"]);
			escenaAnterior = parametros.escenaPadre;
			InicializarMusica(parametros.direcciones["MUSICA_AMBIENTAL"]);
			gestorDeSonidos.Inicializar(new DireccionRecursos(parametros.direcciones["SONIDOS"]));
			selectorImagenes = new IlustradorDeCartas(
				parametrosControl.parametros.direcciones["CARTAS_RECURSO"],
				parametrosControl.parametros.direcciones["CARTAS_DINAMICA"]
			);

			IProveedor<int, CartaBD> proveedorCartas = new LectorCartas(new DireccionRecursos(parametrosControl.parametros.direcciones["CARTAS_DATOS"]));
			cartaGenerador.Inicializar(
				selectorImagenes,
				proveedorCartas,
				new ProveedorColores(
					parametrosControl.parametros.direcciones["COLORES"],
					TipoLector.RECURSOS
				)
			);

			foreach (var sobre in FindObjectsByType<SobreComprar>(
						 FindObjectsInactive.Include,
						 FindObjectsSortMode.None)) {
				sobre.Inicializar(cartaGenerador);
			}
		}

		public void PresionarAbrir() {
			SceneManager.LoadScene("TIENDA ABRIR");
		}

		public void PresionarVolver() {
			SceneManager.LoadScene(escenaAnterior);
		}

	}

}