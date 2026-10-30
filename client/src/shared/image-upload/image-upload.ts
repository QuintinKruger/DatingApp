import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css',
})
export class ImageUpload {
  protected imageSrc = signal<string | ArrayBuffer | null | undefined> (null);
  protected isDragging = false;
  private fileToUpload: File | null = null;
  uploadFile = output<File>();
  loading = input<boolean>(false);

  onDragOver(event: DragEvent) {
    // stop browser from doing these things on the event received 
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }
  onDragLeave(event: DragEvent) {
    // stop browser from doing these things on the event received 
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

   onDrop(event: DragEvent) {
    // stop browser from doing these things on the event received 
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    if (event.dataTransfer?.files.length) {
      // take only 1 image if there are multiple 
      const file = event.dataTransfer.files[0]
      this.previewImage(file);
      this.fileToUpload = file;
    }
  }

   private previewImage(file: File) {
      var reader = new FileReader();
      reader.onload = (e) => this.imageSrc.set(e.target?.result);
      reader.readAsDataURL(file);
    }

    onCancel() {
      this.imageSrc.set(null);
      this.fileToUpload = null;
    }

    onUploadFile() {
      if (this,this.fileToUpload) {
        this.uploadFile.emit(this.fileToUpload)
      }
    }
  
}
