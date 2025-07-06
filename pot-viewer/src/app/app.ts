import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PotList } from './components/pot-list/pot-list';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, PotList],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'pot-viewer';
}
