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
		public ControlBounds controlBounds;
		private ParametrosGlobales parametros;

		public DireccionRecursos carpetaColecciones;
		public GestorDeSobres gestorDeSobres;
		public string escenaAnterior;
		public IProveedor<string, Sprite> selectorImagenes;
		public Cofre cofre;
		public GestorDeSonidos gestorDeSonidos;
		public VentanaControl ventanaControl;
		public CartaGenerador cartaGenerador;

		void Start() {
			parametros = controlBounds.InicializarEscena("GENERAL");
			cofre = new(parametros.direccionesGeneradas["COFRE"], parametros.direccionesGeneradas["COFRE_RECURSOS"]);
			carpetaColecciones = new(parametros.direccionesGeneradas["COLECCIONES"]);
			gestorDeSobres = new(parametros.direccionesGeneradas["SOBRES"]);
			escenaAnterior = parametros.escenaAnterior;
			gestorDeSonidos.Inicializar(new DireccionRecursos(parametros.direccionesGeneradas["SONIDOS"]));
			selectorImagenes = new IlustradorDeCartas(
				new DireccionRecursos(parametros.direccionesGeneradas["CARTAS_RECURSO"]),
				new DireccionDinamica(parametros.direccionesGeneradas["CARTAS_DINAMICA"])
			);

			IProveedor<int, CartaBD> proveedorCartas = new LectorCartas(new DireccionRecursos(parametros.direccionesGeneradas["CARTAS_DATOS"]));
			cartaGenerador.Inicializar(
				selectorImagenes,
				proveedorCartas,
				new ProveedorColores(
					parametros.direccionesGeneradas["COLORES"],
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