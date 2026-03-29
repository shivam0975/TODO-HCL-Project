import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Task, CreateTaskDto, UpdateTaskDto } from '../models/task.model';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

type ApiTaskResponse = Partial<Task> & {
  id?: string;
  _id?: string;
  status?: string | number;
  priority?: string | number;
};

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private readonly apiUrl = `${environment.apiUrl}/api/tasks`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    if (token) headers = headers.set('Authorization', `Bearer ${token}`);
    return headers;
  }

  getTaskById(id: string): Observable<Task> {
    return this.http.get<ApiTaskResponse>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() }).pipe(
      map(task => this.normalizeTask(task)),
      catchError(this.handleError)
    );
  }

  createTask(dto: CreateTaskDto): Observable<Task> {
    const payload = this.normalizeCreateDto(dto);
    return this.http.post<Task>(this.apiUrl, payload, { headers: this.getHeaders() }).pipe(
      catchError(this.handleError)
    );
  }

  updateTask(id: string, dto: UpdateTaskDto): Observable<Task> {
    const payload = this.normalizeUpdateDto(dto);
    return this.http.put<Task>(`${this.apiUrl}/${id}`, payload, { headers: this.getHeaders() }).pipe(
      catchError(this.handleError)
    );
  }

  deleteTask(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() }).pipe(
      catchError(this.handleError)
    );
  }

  getAllTasks(): Observable<Task[]> {
    return this.http.get<ApiTaskResponse[]>(this.apiUrl, { headers: this.getHeaders() }).pipe(
      map(tasks => tasks.map(task => this.normalizeTask(task))),
      catchError(this.handleError)
    );
  }

  toggleTaskStatus(task: Task): Observable<Task> {
    const dto: UpdateTaskDto = {
      title: task.title,
      description: task.description || null,
      status: task.status === 'Pending' ? 'Completed' : 'Pending',
      priority: Number(task.priority) || 0,
      category: task.category || null,
      dueDate: task.dueDate ? new Date(task.dueDate).toISOString() : null
    };
    return this.updateTask(task.id, dto);
  }

  private normalizeCreateDto(dto: CreateTaskDto): CreateTaskDto {
    return {
      title: (dto.title || '').trim().substring(0, 200),
      description: dto.description?.trim().substring(0, 1000) || null,
      category: dto.category?.trim() || null,
      dueDate: dto.dueDate ? new Date(dto.dueDate).toISOString() : null,
      priority: Number(dto.priority) || 0
    };
  }

  private normalizeUpdateDto(dto: UpdateTaskDto): UpdateTaskDto {
    return {
      title: (dto.title || '').trim().substring(0, 200),
      description: dto.description?.trim().substring(0, 1000) || null,
      category: dto.category?.trim() || null,
      dueDate: dto.dueDate ? new Date(dto.dueDate).toISOString() : null,
      priority: Number(dto.priority) || 0,
      status: dto.status || 'Pending'
    };
  }

  private normalizeTask(task: ApiTaskResponse): Task {
    return {
      id: String(task.id ?? task._id ?? ''),
      userId: String(task.userId ?? ''),
      title: String(task.title ?? ''),
      description: (task.description as string | null) ?? null,
      status: this.normalizeStatus(task.status),
      priority: this.normalizePriority(task.priority),
      category: (task.category as string | null) ?? null,
      dueDate: (task.dueDate as string | null) ?? null,
      createdAt: String(task.createdAt ?? new Date().toISOString()),
      updatedAt: String(task.updatedAt ?? new Date().toISOString())
    };
  }

  private normalizeStatus(status: string | number | undefined): Task['status'] {
    if (typeof status === 'number') {
      return status === 1 ? 'Completed' : 'Pending';
    }
    const normalized = String(status ?? '').toLowerCase();
    return normalized === 'completed' ? 'Completed' : 'Pending';
  }

  private normalizePriority(priority: string | number | undefined): Task['priority'] {
    if (typeof priority === 'number') {
      if (priority === 2) return 2;
      if (priority === 1) return 1;
      return 0;
    }
    const normalized = String(priority ?? '').toLowerCase();
    if (normalized === 'high') return 2;
    if (normalized === 'medium') return 1;
    return 0;
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unexpected error occurred';
    if (error.status === 0) errorMessage = 'Cannot connect to server. Ensure API is running';
    else if (error.status === 400) {
      const validationErrors = error.error?.errors;
      if (validationErrors && typeof validationErrors === 'object') {
        const firstKey = Object.keys(validationErrors)[0];
        const firstValue = validationErrors[firstKey];
        if (Array.isArray(firstValue) && firstValue.length > 0) {
          errorMessage = firstValue[0];
        } else {
          errorMessage = 'Invalid request data';
        }
      } else {
        errorMessage = error.error?.title || 'Invalid request data';
      }
    }
    else if (error.status === 401) errorMessage = 'Unauthorized. Please login again.';
    else if (error.status === 404) errorMessage = 'Task not found';
    else if (error.error?.message) errorMessage = error.error.message;
    return throwError(() => new Error(errorMessage));
  }
}