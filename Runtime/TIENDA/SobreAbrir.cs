using System.Collections.Generic;
using UnityEngine;
using Bounds.Persistencia;
using System.Threading.Tasks;
using Bounds.Cofres;
using Ging1991.Interfaces;
using Ging1991.Animaciones.Efectos;
using Bounds.Modulos.Cartas;
using Bounds.Modulos.Cartas.Ilustradores;
using Ging1991.Core.Interfaces;
using Bounds.Persistencia.Datos;
using Bounds.Visuales;
using Bounds.Modulos.Cartas.Persistencia.Datos;
using Ging1991.Interfaces.Salida;
using Bounds.Cartas;

namespace Bounds.Tienda {

	public class SobreAbrir : MonoBehaviour {

		public Coleccion coleccion;
		public string nombre;
		public int cantidad;
		public IlustradorDeCartas ilustrador;
		public ControlTiendaAbrir sobreControl;
		public ContenedorID contenedorID;
		public MarcoConTexto cantidadOBJ;

		public void Inicializar(CartaGenerador cartaGenerador, Coleccion coleccion) {
			sobreControl = FindAnyObjectByType<ControlTiendaAbrir>();
			this.coleccion = coleccion;

			contenedorID.generador = cartaGenerador;
			contenedorID.MostrarCartaID(coleccion.emblema.cartaID, coleccion.emblema.imagen, "N");
			contenedorID.primitiva.SetTituloTexto($"{coleccion.titulo}", 0);
			contenedorID.primitiva.SetTituloTexto($"{EstablecerPosesion()}", 1);
			EstablecerCantidad();
		}


		private void EstablecerCantidad() {
			GestorDeSobres lector = sobreControl.gestorDeSobres;
			cantidad = lector.GetCantidad(coleccion.codigo);
			if (cantidad == 0) {
				ControlTiendaAbrir control = GameObject.Find("Control").GetComponent<ControlTiendaAbrir>();
				control.Remover(gameObject);
				Destroy(gameObject);
			}
			cantidadOBJ.SetTexto($"{cantidad}");
		}


		void OnMouseDown() {
			MostrarRecompensas();

			GameObject.Find("GestorEfectosVisuales").GetComponent<GestorEfectosVisuales>().Animar("EXPLOSION");
			GestorDeSobres lector = sobreControl.gestorDeSobres;
			lector.SetCantidad(coleccion.codigo, lector.GetCantidad(coleccion.codigo) - 1);
			EstablecerCantidad();
		}


		private GameObject TraerRecompensa(int numero) {
			return GameObject.Find("Carta" + numero);
		}


		private void MostrarRecompensas() {
			Sobre sobre = coleccion.CrearSobre();
			int posY = -300;
			int posX = 0;
			EstablecerRecompensa(TraerRecompensa(1), sobre.comunes[0], new Vector3(posX, posY, 0), "N");
			EstablecerRecompensa(TraerRecompensa(2), sobre.comunes[1], new Vector3(posX + 160, posY, 0), "N");
			EstablecerRecompensa(TraerRecompensa(3), sobre.comunes[2], new Vector3(posX + 320, posY, 0), "N");
			EstablecerRecompensa(TraerRecompensa(4), sobre.infrecuentes[0], new Vector3(posX + 480, posY, 0), "PLA");
			EstablecerRecompensa(TraerRecompensa(5), sobre.infrecuentes[1], new Vector3(posX + 640, posY, 0), "PLA");
			EstablecerRecompensa(TraerRecompensa(6), sobre.rara, new Vector3(posX + 800, posY, 0), sobre.rarezaSobre);

			ControlTiendaAbrir sobreControl = FindAnyObjectByType<ControlTiendaAbrir>();
			Task.Run(() => {
				sobreControl.Guardar();
			});
		}


		private void EstablecerRecompensa(GameObject recompensa, CartaColeccionBD carta, Vector3 posicionFinal, string rareza) {
			ControlTiendaAbrir sobreControl = FindAnyObjectByType<ControlTiendaAbrir>();
			sobreControl.AgregarCarta(carta.cartaID, carta.imagen, rareza);

			recompensa.GetComponentInChildren<CartaImagenID>().MostrarCartaID(carta.cartaID, carta.imagen, rareza);
			Vector3 posicionInicial = transform.localPosition;
			recompensa.GetComponent<MoverVelocidad>().Inicializar(
				posicionInicial,
				posicionFinal,
				20,
				accionFinal: new RecorridoTerminado(recompensa, Color.white)
			);
		}


		public class RecorridoTerminado : IEjecutable {

			private readonly GameObject recompensa;
			private Color color;

			public RecorridoTerminado(GameObject recompensa, Color color) {
				this.recompensa = recompensa;
				this.color = color;
			}

			public void Ejecutar() {
				//recompensa.GetComponentInChildren<EfectoVisual>().Animar("REVITALIZAR", color);
			}

		}


		protected void InicializarImagen(IProveedor<int, CartaBD> proveedorCartas) {
			//EstablecerImagen(proveedorCartas, ilustrador, tintero, coleccion.emblema.cartaID, coleccion.emblema.imagen);
		}


		protected void EstablecerImagen(IProveedor<int, CartaBD> proveedorCartas, IlustradorDeCartas ilustrador, int cartaID, string imagen) {
			//GetComponent<ContenedorDeCartas>().Inicializar(proveedorCartas, ilustrador, tintero, cartaID, imagen);
		}


		protected string EstablecerPosesion() {
			Cofre cofre = ControlTiendaAbrir.Instancia.cofre;
			List<CartaColeccionBD> cartasColeccion = coleccion.GetListaCompleta();
			List<string> cartasID = new();
			foreach (var cartaColeccion in cartasColeccion) {
				cartasID.Add(GetCodigoFormato(cartaColeccion.cartaID, cartaColeccion.imagen));
			}
			int cartasObtenidas = cofre.GetCantidadCartasPorColeccion(cartasID);
			int cartasTotales = cartasColeccion.Count;
			int porcentaje = (int)(((float)cartasObtenidas / cartasTotales) * 100);
			return $"{cartasObtenidas}/{cartasTotales} ({porcentaje}%)";
		}


		protected string GetCodigoFormato(int cartaID, string imagen) {
			if (cartaID < 10)
				return $"00{cartaID}_{imagen}";
			if (cartaID < 100)
				return $"0{cartaID}_{imagen}";
			return $"{cartaID}_{imagen}";
		}


	}

}