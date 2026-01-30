using System.Collections.Generic;
using UnityEngine;
using Bounds.Persistencia;
using System.Threading.Tasks;
using Bounds.Cofres;
using Ging1991.Interfaces;
using Ging1991.Animaciones.Efectos;
using Bounds.Modulos.Cartas;
using Bounds.Modulos.Cartas.Tinteros;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Persistencia;
using Ging1991.Core.Interfaces;
using Bounds.Persistencia.Datos;

namespace Bounds.Tienda {

	public class SobreAbrir : MonoBehaviour {

		public Coleccion coleccion;
		public string nombre;
		public int cantidad;
		public GameObject nombreOBJ;
		public GameObject cantidadOBJ;
		public GameObject posesionOBJ;
		private ITintero tintero;
		public IlustradorDeCartas ilustrador;
		public ControlTiendaAbrir sobreControl;

		public void Iniciar(Coleccion coleccion, IlustradorDeCartas ilustrador, ITintero tintero) {
			sobreControl = FindAnyObjectByType<ControlTiendaAbrir>();
			this.coleccion = coleccion;
			this.ilustrador = ilustrador;
			this.tintero = tintero;
			EstablecerCantidad();
			nombreOBJ.GetComponent<MarcoConTexto>().SetTexto(coleccion.titulo);
			posesionOBJ.GetComponent<MarcoConTexto>().SetTexto(EstablecerPosesion());
			InicializarImagen();
			DatosDeCartas.Instancia.Inicializar();
		}


		private void EstablecerCantidad() {
			GestorDeSobres lector = sobreControl.gestorDeSobres;
			cantidad = lector.GetCantidad(coleccion.codigo);
			if (cantidad == 0) {
				ControlTiendaAbrir control = GameObject.Find("Control").GetComponent<ControlTiendaAbrir>();
				control.Remover(gameObject);
				Destroy(gameObject);
			}
			cantidadOBJ.GetComponent<MarcoConTexto>().SetTexto($"{cantidad}");
		}


		void OnMouseDown() {
			MostrarRecompensas();

			//GameObject.Find("GestorVisual").GetComponent<GestorVisual>().Animar("GOLPE");
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

			recompensa.GetComponentInChildren<CartaFrente>().Inicializar(DatosDeCartas.Instancia, ilustrador, tintero);
			recompensa.GetComponentInChildren<CartaFrente>().Mostrar(carta.cartaID, carta.imagen, rareza);
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


		protected void InicializarImagen() {
			EstablecerImagen(ilustrador, tintero, coleccion.emblema.cartaID, coleccion.emblema.imagen);
		}


		protected void EstablecerImagen(IlustradorDeCartas ilustrador, ITintero tintero, int cartaID, string imagen) {
			GetComponent<ContenedorDeCartas>().Inicializar(ilustrador, tintero, cartaID, imagen);
		}


		protected string EstablecerPosesion() {
			Cofre cofre = new Cofre();
			List<CartaColeccionBD> cartas = coleccion.GetListaCompleta();
			List<int> cartasID = new List<int>();
			foreach (var carta in cartas) {
				if (!cartasID.Contains(carta.cartaID))
					cartasID.Add(carta.cartaID);
			}
			int cartasObtenidas = cofre.GetCantidadCartasDiferentes(cartasID);
			int cartasTotales = cartas.Count;
			int porcentaje = (int)(((float)cartasObtenidas / cartasTotales) * 100);
			return $"{cartasObtenidas}/{cartasTotales} ({porcentaje}%)";
		}


	}

}