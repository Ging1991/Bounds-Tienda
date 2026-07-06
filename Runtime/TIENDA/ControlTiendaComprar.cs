using Bounds.Cartas;
using Bounds.Cofres;
using Bounds.Entrenamiento;
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
		public Billetera billetera;
		public ParametrosControl parametrosControl;
		public DireccionRecursos carpetaColecciones;
		public GestorDeSobres gestorDeSobres;
		public string escenaAnterior;
		public MusicaDeFondo musicaDeFondo;
		public IProveedor<string, Sprite> selectorImagenes;
		public Cofre cofre;
		public GestorDeSonidos gestorDeSonidos;
		public ControlUIBounds personalizarUI;
		public VentanaControl ventanaControl;
		public CartaGenerador cartaGenerador;

		void Start() {
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;
			personalizarUI.Personalizar(parametros.direcciones["SISTEMA"], parametros.direcciones["COLORES"]);

			configuracion = new(parametros.direcciones["CONFIGURACION"]);
			billetera = new(parametros.direcciones["BILLETERA"]);
			cofre = new(parametros.direcciones["COFRE"], parametros.direcciones["COFRE_RECURSOS"]);
			carpetaColecciones = new(parametros.direcciones["COLECCIONES"]);
			gestorDeSobres = new(parametros.direcciones["SOBRES"]);
			escenaAnterior = parametros.escenaPadre;
			musicaDeFondo.Inicializar(parametros.direcciones["MUSICA_DE_TIENDA"]);
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