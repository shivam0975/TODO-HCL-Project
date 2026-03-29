import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <header class="header">
      <div class="header-left">
        <h1 class="page-title">My Tasks</h1>
        <p class="page-subtitle">{{ today | date:'EEEE, MMMM d, y' }}</p>
      </div>
      <div class="header-right">
        <a routerLink="/tasks/new" class="btn-add">
          <span>+</span> New Task
        </a>
      </div>
    </header>
  `,
  styles: [`
    .header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 1.25rem 2rem;
      border-bottom: 1px solid rgba(255,255,255,0.06);
      background: #0f0f1a;
      flex-shrink: 0;
    }
    .page-title {
      font-size: 1.5rem;
      font-weight: 700;
      color: #fff;
      margin: 0;
    }
    .page-subtitle {
      color: #7070a0;
      font-size: 0.8rem;
      margin: 0.15rem 0 0 0;
    }
    .btn-add {
      display: flex;
      align-items: center;
      gap: 0.4rem;
      background: linear-gradient(135deg, #6c63ff, #a78bfa);
      color: white;
      border: none;
      border-radius: 10px;
      padding: 0.5rem 1rem;
      font-weight: 600;
      font-size: 0.9rem;
      cursor: pointer;
      text-decoration: none;
      transition: all 0.2s;
    }
    .btn-add:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 16px rgba(108, 99, 255, 0.3);
    }
  `]
})
export class HeaderComponent implements OnInit {
  today = new Date();

  ngOnInit(): void {}
}
