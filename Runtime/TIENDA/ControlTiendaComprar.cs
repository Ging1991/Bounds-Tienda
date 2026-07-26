using Bounds.Cartas;
using Bounds.Cofres;
using Bounds.Modulos.Cartas.Persistencia;
using Bounds.Modulos.Cartas.Persistencia.Datos;
using Bounds.Musica;
using Bounds.Persistencia;
using Bounds.Persistencia.proveedores;
using Bounds.Sistema;
using Bounds.Sistema.Ilustradores;
using Bounds.Sistema.Parametros;
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

		public ControlParametros parametrosControl;
		public DireccionRecursos carpetaColecciones;
		public GestorDeSobres gestorDeSobres;
		public string escenaAnterior;
		public IProveedor<string, Sprite> selectorImagenes;
		public Cofre cofre;
		public GestorDeSonidos gestorDeSonidos;
		public ControlUIBounds personalizarUI;
		public VentanaControl ventanaControl;
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

			cofre = new(parametros.direccionesGeneradas["COFRE"], parametros.direccionesGeneradas["COFRE_RECURSOS"]);
			carpetaColecciones = new(parametros.direccionesGeneradas["COLECCIONES"]);
			gestorDeSobres = new(parametros.direccionesGeneradas["SOBRES"]);
			escenaAnterior = parametros.escenaAnterior;
			gestorDeSonidos.Inicializar(new DireccionRecursos(parametros.direccionesGeneradas["SONIDOS"]));
			selectorImagenes = new IlustradorDeCartas(
				new DireccionRecursos(parametrosControl.parametros.direccionesGeneradas["CARTAS_RECURSO"]),
				new DireccionDinamica(parametrosControl.parametros.direccionesGeneradas["CARTAS_DINAMICA"])
			);

			IProveedor<int, CartaBD> proveedorCartas = new LectorCartas(new DireccionRecursos(parametrosControl.parametros.direccionesGeneradas["CARTAS_DATOS"]));
			cartaGenerador.Inicializar(
				selectorImagenes,
				proveedorCartas,
				new ProveedorColores(
					parametrosControl.parametros.direccionesGeneradas["COLORES"],
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