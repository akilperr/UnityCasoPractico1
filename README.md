# AR Book Tropes 📚✨

AR Book Tropes es una aplicación de Realidad Aumentada que permite reconocer portadas de libros y mostrar información contextual sobre ellos directamente sobre la imagen detectada.

La aplicación está enfocada principalmente en libros de **fantasía, romance y romantasy**, mostrando los **tópicos narrativos** presentes en cada historia y otra información relevante para el lector.

---

# Demo
<div align="center">
  <table>
    <tr>
      <td><img src="imagenes/topicos.png" alt="Topicos del libro detectado" width="400"/></td>
      <td><img src="imagenes/panel de info.png" alt="Información extra del libro detectado" width="400"/></td>
    </tr>
  </table>
</div>div>

---

# Funcionalidades

- 📖 **Reconocimiento de portadas de libros** mediante *image tracking*.
- 🏷 **Visualización de tópicos narrativos** flotando sobre el libro detectado.
- ℹ **Panel de información adicional** con datos del libro.
- 📸 **Captura de imágenes** para guardar el libro detectado en la galería.
- 📚 **Información sobre la serie** (orden de lectura, estado, libros relacionados).

---

# Información mostrada

La aplicación combina información obtenida de dos fuentes.

## Datos almacenados en JSON

- Título del libro
- Tópicos narrativos
- Serie a la que pertenece
- Tipo de serie (trilogía, bilogía, saga...)
- Orden dentro de la serie
- Estado de la serie
- Libros anteriores y posteriores
- Otros libros relacionados
- Color asociado a las etiquetas

## Datos obtenidos desde la API de Google Books

- Autor
- Editorial
- Fecha de publicación
- Categoría del libro

---

# Tecnologías utilizadas

- **Unity**
- **AR Foundation**
- **ARCore**
- **Google Books API**
- **Native Gallery (Unity plugin)**
- **JSON** para almacenamiento local de datos

---

# Funcionamiento del sistema

1. El usuario enfoca la portada de un libro con la cámara del dispositivo.
2. El sistema utiliza **tracking de imágenes** para reconocer la portada.
3. Si el libro está registrado en la base de datos:
   - Aparecen **etiquetas flotantes con los tópicos narrativos**.
4. El usuario puede:
   - Abrir un **panel de información adicional**.
   - **Capturar una imagen** del libro detectado.

Si hay varios libros visibles, el sistema prioriza el **libro más centrado en la pantalla**.

<div align="center">
  <img src="imagenes/el rey malvado.png" width="900">
</div>div>


---

# Caso de uso

Esta aplicación está pensada para facilitar el descubrimiento de libros cuando el lector se encuentra en contextos como:

- Librerías
- Bibliotecas
- Exploración de nuevos títulos

Muchos lectores utilizan los **tópicos narrativos** (dragones, faes, enemies to lovers, etc.) para encontrar historias que se ajusten a sus preferencias. AR Book Tropes permite visualizar esta información directamente sobre el libro físico.

---

# Limitaciones

- La base de datos actual contiene **12 libros**, por lo que solo se reconocen portadas previamente registradas.
- La **API de Google Books** no siempre proporciona información completa.
- El sistema depende de la **calidad del tracking de imágenes**.
- Portadas visualmente similares podrían generar confusiones en el reconocimiento.

---

# Futuras mejoras

- Ampliar la base de datos de libros.
- Integrar una base de datos externa más completa.
- Mejorar la interfaz visual.
- Añadir soporte para más géneros literarios.

---

# Autor

**Lucía Liu Wang**

Proyecto realizado para la asignatura:

**Sistemas Interactivos e Inmersivos – 2026**
